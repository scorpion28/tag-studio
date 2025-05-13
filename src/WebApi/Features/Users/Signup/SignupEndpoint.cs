using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Identity;
using TagStudio.WebApi.Infrastructure.Identity;

namespace TagStudio.WebApi.Features.Users;

public class SignupEndpoint(UserManager<AppUser> userManager)
    : Endpoint<SignupRequest>
{
    public override void Configure()
    {
        Post("/auth/signup");
        AllowAnonymous();

        Summary(s => s.Summary = "Sign Up");
        Description(x => x.AutoTagOverride("Auth"));
    }

    public override async Task HandleAsync(SignupRequest req, CancellationToken ct)
    {
        var user = new AppUser();

        await userManager.SetUserNameAsync(user, req.Email);
        await userManager.SetEmailAsync(user, req.Email);

        var result = await userManager.CreateAsync(user, req.Password);

        if (!result.Succeeded)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await SendOkAsync(ct);
    }
}