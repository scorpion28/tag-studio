using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TagStudio.WebApi.Infrastructure.Data;
using TagStudio.WebApi.Infrastructure.Identity;

namespace TagStudio.WebApi.Features.Authentication;

public class JwtTokenService(
    ApplicationDbContext context,
    IConfiguration configuration) : ITokenService
{
    public TokenData GenerateTokens(AppUser user)
    {
        var (accessToken, expiresAtUtc) = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        
        var expiresAtUnixTimestamp = new DateTimeOffset(expiresAtUtc).ToUnixTimeMilliseconds();
        
        return new TokenData(accessToken, refreshToken, expiresAtUnixTimestamp);
    }

    public async Task<UserRefreshToken?> FindRefreshTokenAsync(string token)
    {
        return await context.RefreshTokens.FindAsync(token);
    }

    public async Task SetRefreshTokenAsync(string token, AppUser user)
    {
        ArgumentException.ThrowIfNullOrEmpty(token);

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationInMinutes"));
        user.UpdateRefreshToken(token, expiresAt);

        await context.SaveChangesAsync();
    }

    public async Task RevokeRefreshTokenAsync(UserRefreshToken token)
    {
        context.RefreshTokens.Remove(token);

        await context.SaveChangesAsync();
    }

    private (string Token, DateTime ExpiresAtUtc) GenerateAccessToken(AppUser user)
    {
        var secretKey = configuration["Jwt:Key"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var expirationInMinutes = configuration.GetValue<int>("Jwt:ExpirationInMinutes");
        var utcNow = DateTime.UtcNow;
        var expiresAt = utcNow.AddMinutes(expirationInMinutes);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = configuration.GetValue<string>("Jwt:Audience"),
            IssuedAt = utcNow,
            NotBefore = utcNow,
            Expires = expiresAt,
            Issuer = configuration.GetValue<string>("Jwt:Issuer"),
            SigningCredentials = credentials,
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!)
            ])
        };

        var handler = new JsonWebTokenHandler();
        var accessToken = handler.CreateToken(tokenDescriptor);

        return (accessToken, expiresAt);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}