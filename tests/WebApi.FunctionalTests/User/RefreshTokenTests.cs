using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TagStudio.Identity.Data;
using TagStudio.Identity.Domain;
using TagStudio.Identity.Features;

namespace TagStudio.WebApi.FunctionalTests.User;

public class RefreshTokenTests(TagStudioFactory appFactory) : TestsBase(appFactory)
{
    private readonly TagStudioFactory _appFactory = appFactory;
    private readonly HttpClient _httpClient = appFactory.CreateClient();

    [Fact]
    public async Task RefreshToken_ShouldReturnNewTokenData_WhenValidRefreshToken()
    {
        var user = await _appFactory.CreateUserAsync();
        var refreshToken = await CreateRefreshToken(user.Id);
        var request = new { RefreshToken = refreshToken };

        var refreshResponse = await _httpClient.PostAsJsonAsync("/auth/refresh", request);

        refreshResponse.IsSuccessStatusCode.ShouldBeTrue();
        var tokenData = await refreshResponse.Content.ReadFromJsonAsync<TokenData>();
        tokenData.ShouldNotBeNull();
        tokenData.RefreshToken.ShouldNotBeNullOrWhiteSpace();
        tokenData.AccessToken.ShouldNotBeNullOrWhiteSpace();
        tokenData.ExpiresAtUtc.ShouldBeGreaterThan(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
    }

    [Fact]
    public async Task RefreshToken_ShouldReturn401Unauthorized_WhenRefreshTokenNotExist()
    {
        // Arrange
        const string token = "non-existent-token";
        var request = new { RefreshToken = token };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/auth/refresh", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        // Verify problem details
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.ShouldNotBeNull();
        problemDetails.Title.ShouldBe("Unauthorized");
        problemDetails.Detail.ShouldBe("Token is invalid");
    }

    [Fact]
    public async Task RefreshToken_ShouldReturn401Unauthorized_WhenRefreshTokenExpired()
    {
        // Arrange
        var user = await _appFactory.CreateUserAsync();
        var dayBeforeNow = DateTimeOffset.UtcNow.AddDays(-1);
        var refreshToken = await CreateRefreshToken(user.Id, dayBeforeNow);
        var request = new { RefreshToken = refreshToken };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/auth/refresh", request);

        // Assert 
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        // Verify problem details
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.ShouldNotBeNull();
        problemDetails.Title.ShouldBe("Unauthorized");
        problemDetails.Detail.ShouldBe("Token is expired");
    }

    /// Helper method that creates a <see cref="UserRefreshToken"/> and stores it in the database
    private async Task<string> CreateRefreshToken(Guid userId, DateTimeOffset? expiresAt = null)
    {
        expiresAt ??= DateTimeOffset.UtcNow.AddMinutes(30);

        var refreshToken = Guid.NewGuid().ToString();
        var userToken = new UserRefreshToken(refreshToken, userId, expiresAt.Value);

        await SeedRefreshToken(userToken);

        return refreshToken;
    }

    private async Task SeedRefreshToken(UserRefreshToken token)
    {
        await using var scope = _appFactory.Services.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

        await db.RefreshTokens.AddAsync(token);

        await db.SaveChangesAsync();
    }
}