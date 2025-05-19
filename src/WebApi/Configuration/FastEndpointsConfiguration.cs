using System.Security.Claims;
using FastEndpoints;
using TagStudio.WebApi.Common.Models;
using TagStudio.WebApi.Features.Authentication;

// ReSharper disable once CheckNamespace
internal static class FastEndpointsConfiguration
{
    public static void ApplyDefaultPolicies(this EndpointOptions endpoints)
    {
        endpoints.Configurator = ep =>
        {
            var allowAnonymous = ep.Routes[0].StartsWith("/auth");
            if (!allowAnonymous)
            {
                ep.Policy(pb => pb.RequireCurrentUser());
            }
        };
    }
    
    public static void BindUserIdFromClaims(this BindingOptions options)
    {
        options.Modifier = (req, _, ctx, _) =>
        {
            if (req is not RequestWithUserId r) return;

            var stringId = ctx.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (stringId != null && Guid.TryParse(stringId, out var id))
            {
                r.UserId = id;
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        };
    }

}