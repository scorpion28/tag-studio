using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using TagStudio.WebApi.Features.Users;

namespace TagStudio.WebApi.FunctionalTests.User;

public sealed class UserTests(TagStudioFactory appFactory) : TestsBase(appFactory)
{
    private readonly HttpClient _httpClient = appFactory.CreateClient();

    [Fact]
    public async Task SignUp_ShouldReturnOk_WhenValidCredentials()
    {
        var signupData = new SignupRequest { Email = "user@example.com", Password = "@1StrongPassword" };

        var response = await _httpClient.PostAsJsonAsync("/auth/signup", signupData);

        response.IsSuccessStatusCode.ShouldBeTrue();
    }

    [Fact]
    public async Task Login_ShouldReturnValidTokenAndUserData_WhenUserExists()
    {
        // Arrange
        var credentials = new { Email = "user@example.com", Password = "@1StrongPassword" };
        var signupResponse = await _httpClient.PostAsJsonAsync("/auth/signup", credentials);

        // Verify signup is successful 
        Assert.True(signupResponse.IsSuccessStatusCode);

        // Act
        var loginResponse = await _httpClient.PostAsJsonAsync("/auth/login", credentials);
        var loginResponseData = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        // Verify response not empty 
        loginResponse.IsSuccessStatusCode.ShouldBeTrue();
        loginResponseData.ShouldNotBeNull();

        // Assert
        var tokenData = loginResponseData.Jwt;
        tokenData.AccessToken.ShouldNotBeNullOrWhiteSpace();
        tokenData.RefreshToken.ShouldNotBeNullOrEmpty();
        tokenData.ExpiresAtUtc.ShouldBeGreaterThan(0);
        loginResponseData.User.Name.ShouldBe(credentials.Email);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenInvalidRequestBody()
    {
        var invalidRequestBody = new { InvalidProperty = "username" };

        var response = await _httpClient.PostAsJsonAsync("/auth/login", invalidRequestBody);

        // Assert 
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        // Verify error message
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails.ShouldNotBeNull();
        problemDetails.Errors.ShouldContainKey("email");
        problemDetails.Errors["email"].ShouldContain("Email cannot be empty");
        problemDetails.Errors.ShouldContainKey("email");
        problemDetails.Errors["password"].ShouldContain("Password cannot be empty");
    }

    [Fact]
    public async Task SignUp_ShouldReturnBadRequest_WhenInvalidEmailOrPassword()
    {
        var wrongRequestBody = new LoginRequest { Email = "wrongemail", Password = "weakpassword" };

        var response = await _httpClient.PostAsJsonAsync("/auth/signup", wrongRequestBody);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        // Verify error message
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails.ShouldNotBeNull();
        problemDetails.Errors.ShouldContainKey("email");
        problemDetails.Errors["email"].ShouldContain("Email is invalid");
        problemDetails.Errors.ShouldContainKey("password");
        problemDetails.Errors["password"]
            .ShouldContain(
                "Password must be a mix of lowercase and uppercase letters, numbers and special characters");
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
    {
        var credentials = new LoginRequest { Email = "user@example.com", Password = "@1StrongPassword" };

        var response = await _httpClient.PostAsJsonAsync("/auth/login", credentials);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}