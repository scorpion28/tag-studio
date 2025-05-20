using System.Security.Claims;

namespace TagStudio.Shared.User;

/// An entity to expose current user information
public class CurrentUser
{
    public Guid Id { get; set; } = Guid.Empty;

    public ClaimsPrincipal Principal { get; set; } = null!;
}