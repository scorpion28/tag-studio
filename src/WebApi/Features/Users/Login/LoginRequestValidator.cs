using FastEndpoints;

namespace TagStudio.WebApi.Features.Users;

public class LoginRequestValidator : Validator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .ValidEmail();

        RuleFor(p => p.Password)
            .ValidPassword();
    }
}