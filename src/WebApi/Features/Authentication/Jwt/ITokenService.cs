using TagStudio.WebApi.Infrastructure.Identity;

namespace TagStudio.WebApi.Features.Authentication;

public interface ITokenService
{
    TokenData GenerateTokens(AppUser user);

    Task<UserRefreshToken?> FindRefreshTokenAsync(string token);

    Task SetRefreshTokenAsync(string token, AppUser user);

    Task RevokeRefreshTokenAsync(UserRefreshToken token);
}