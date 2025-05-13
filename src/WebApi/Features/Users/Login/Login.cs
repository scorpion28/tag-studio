using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using TagStudio.WebApi.Features.Authentication;
using TagStudio.WebApi.Infrastructure.Identity;

namespace TagStudio.WebApi.Features.Users;

public class LoginEndpoint(SignInManager<AppUser> signInManager, JwtTokenProvider tokenProvider)
    : Endpoint<LoginRequest, Results<Ok<LoginResponse>, EmptyHttpResult, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();

        Summary(s => s.Summary = "Log In");
        Description(x => { x.AutoTagOverride("Auth"); });
    }

    public override async Task<Results<Ok<LoginResponse>, EmptyHttpResult, ProblemHttpResult>> ExecuteAsync(
        LoginRequest req, CancellationToken ct)
    {
        var user = await signInManager.UserManager.FindByEmailAsync(req.Email);
        if (user is null)
        {
            return TypedResults.Problem(detail: "User doesn't exist",
                statusCode: StatusCodes.Status401Unauthorized);
        }

        var loginResult = await signInManager.CheckPasswordSignInAsync(user, req.Password, lockoutOnFailure:false);

        if (!loginResult.Succeeded)
        {
            return TypedResults.Problem(loginResult.ToString(), statusCode: StatusCodes.Status401Unauthorized);
        }

        var jwtData = tokenProvider.Create(user);
        var userInfo = new UserInfo(user.UserName ?? throw new NullReferenceException(nameof(user)));

        return TypedResults.Ok(new LoginResponse(jwtData, userInfo));
    }
}