using FastEndpoints;

namespace TagStudio.WebApi.Features.Users;

public class SignupRequestValidator : Validator<SignupRequest>
{
    public SignupRequestValidator()
    {
        RuleFor(x => x.Email)
            .ValidEmail();

        RuleFor(x => x.Password)
            .ValidPassword();
    }
}