using System.ComponentModel.DataAnnotations;

namespace TagStudio.WebApi.Infrastructure.Identity;

public class UserRefreshToken(string token, Guid userId, DateTimeOffset expiresAt)
{
    [MaxLength(128)]
    [Key]
    public string Token { get; init; } = token;

    public DateTimeOffset ExpiresAt { get; init; } = expiresAt;
    public bool Expired => ExpiresAt < DateTimeOffset.UtcNow;

    public Guid UserId { get; init; } = userId;
    public AppUser User { get; init; } = null!;
}