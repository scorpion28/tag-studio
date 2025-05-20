using FastEndpoints;

namespace TagStudio.Identity.Features;

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