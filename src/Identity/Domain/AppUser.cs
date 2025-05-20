using Microsoft.AspNetCore.Identity;

namespace TagStudio.Identity.Domain;

public class AppUser : IdentityUser<Guid>
{
    public List<UserRefreshToken> RefreshTokens { get; } = [];

    public void UpdateRefreshToken(string token, DateTimeOffset expiration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token, nameof(token));

        // For now only one token per user is allowed
        RefreshTokens.Clear();

        var newToken = new UserRefreshToken(token, Id, expiration);
        RefreshTokens.Add(newToken);
    }
}