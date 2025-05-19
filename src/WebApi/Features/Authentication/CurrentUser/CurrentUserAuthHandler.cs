using Microsoft.AspNetCore.Authorization;

namespace TagStudio.WebApi.Features.Authentication;

public static class AuthorizationHandlerExtensions
{
    public static AuthorizationBuilder AddCurrentUserHandler(this AuthorizationBuilder builder)
    {
        builder.Services.AddScoped<IAuthorizationHandler, CheckCurrentUserAuthHandler>();
        return builder;
    }

   /// <summary>
   /// Adds authorization handler that verifies that the user exists even if there's a valid token 
   /// </summary>
    public static AuthorizationPolicyBuilder RequireCurrentUser(this AuthorizationPolicyBuilder builder)
    {
        return builder.RequireAuthenticatedUser()
            .AddRequirements(new CheckCurrentUserRequirement());
    }

    private class CheckCurrentUserRequirement : IAuthorizationRequirement;

    private class CheckCurrentUserAuthHandler(CurrentUser currentUser)
        : AuthorizationHandler<CheckCurrentUserRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CheckCurrentUserRequirement requirement)
        {
            if (currentUser.User is not null)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}