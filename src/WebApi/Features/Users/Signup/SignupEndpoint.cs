using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using TagStudio.WebApi.Infrastructure.Identity;

namespace TagStudio.WebApi.Features.Users;

public class SignupEndpoint(UserManager<AppUser> userManager)
    : Endpoint<SignupRequest, Results<Ok, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post("/auth/signup");
        AllowAnonymous();

        Summary(s => s.Summary = "Sign Up");
        Description(x =>
        {
            x.AutoTagOverride("Auth");
            
            x.ProducesProblemDetails(StatusCodes.Status401Unauthorized);
            x.ProducesProblemDetails(StatusCodes.Status409Conflict);
        });
    }

    public override async Task<Results<Ok, ProblemHttpResult>> ExecuteAsync(SignupRequest req, CancellationToken ct)
    {
        var userAlreadyExists = await userManager.FindByNameAsync(req.Email) is not null;
        if (userAlreadyExists)
        {
            return TypedResults.Problem("Email is already taken", statusCode: StatusCodes.Status409Conflict);
        }

        var user = new AppUser();

        await userManager.SetUserNameAsync(user, req.Email);
        await userManager.SetEmailAsync(user, req.Email);

        var result = await userManager.CreateAsync(user, req.Password);

        if (!result.Succeeded)
        {
            return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
        }

        return TypedResults.Ok();
    }
}