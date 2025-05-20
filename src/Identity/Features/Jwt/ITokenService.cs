using TagStudio.Identity.Domain;

namespace TagStudio.Identity.Features;

public interface ITokenService
{
    TokenData GenerateTokens(AppUser user);

    Task<UserRefreshToken?> FindRefreshTokenAsync(string token);

    Task SetRefreshTokenAsync(string token, AppUser user);

    Task RevokeRefreshTokenAsync(UserRefreshToken token);
}