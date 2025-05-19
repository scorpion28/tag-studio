using System.Security.Claims;
using TagStudio.WebApi.Infrastructure.Identity;

namespace TagStudio.WebApi.Features.Authentication;

/// A scoped service that exposes the current user information
public class CurrentUser
{
    public AppUser? User { get; set; }
    public ClaimsPrincipal Principal { get; set; } = null!;

    private string FindId() => Principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public Guid GetId() => new(FindId());
}