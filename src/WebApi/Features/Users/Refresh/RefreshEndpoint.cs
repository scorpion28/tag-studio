using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using TagStudio.WebApi.Features.Authentication;
using TagStudio.WebApi.Infrastructure.Identity;

namespace TagStudio.WebApi.Features.Users.Refresh;

public class RefreshEndpoint(ITokenService tokenService, UserManager<AppUser> userManager, ILogger<RefreshEndpoint> logger)
    : Endpoint<RefreshRequest, Results<Ok<TokenData>, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post("/auth/refresh");
        AllowAnonymous();

        Summary(s => s.Summary = "Refresh Token");
        Description(x =>
        {
            x.AutoTagOverride("Auth");
            x.ProducesProblemDetails(StatusCodes.Status401Unauthorized);
        });
    }

    public override async Task<Results<Ok<TokenData>, ProblemHttpResult>> ExecuteAsync(RefreshRequest req,
        CancellationToken ct)
    {
        var userToken = await tokenService.FindRefreshTokenAsync(req.RefreshToken);

        if (userToken is null)
        {
            return TypedResults.Problem("Token is invalid", statusCode: StatusCodes.Status401Unauthorized);
        }

        if (userToken.Expired)
        {
            return TypedResults.Problem("Token is expired", statusCode: StatusCodes.Status401Unauthorized);
        }

        var user = await userManager.FindByIdAsync(userToken.UserId.ToString());
        if (user is null)
        {
            return TypedResults.Problem("Token is invalid", statusCode: StatusCodes.Status401Unauthorized);
        }

        await tokenService.RevokeRefreshTokenAsync(userToken);
        logger.LogInformation("Refresh token revoked"); 

        var tokenData = tokenService.GenerateTokens(userToken.User);
        await tokenService.SetRefreshTokenAsync(tokenData.RefreshToken, userToken.User);
        
        return TypedResults.Ok(tokenData);
    }
}