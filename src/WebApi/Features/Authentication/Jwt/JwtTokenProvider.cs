using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TagStudio.WebApi.Infrastructure.Identity;

namespace TagStudio.WebApi.Features.Authentication;

public class JwtTokenProvider(IConfiguration configuration)
{
    public TokenData Create(AppUser user)
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
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!)
            ])
        };

        var handler = new JsonWebTokenHandler();
        var accessToken = handler.CreateToken(tokenDescriptor);

        return new TokenData(accessToken, RefreshToken: "", new DateTimeOffset(expiresAt).ToUnixTimeMilliseconds());
    }
}