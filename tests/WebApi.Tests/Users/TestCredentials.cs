using FastEndpoints;
using TagStudio.Identity.Features;

namespace WebApi.Tests.Users;

public class TestCredentials
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class TestCredentialsValidator : Validator<TestCredentials>
{
    public TestCredentialsValidator()
    {
        RuleFor(x => x.Email)
            .ValidEmail();

        RuleFor(x => x.Password)
            .ValidPassword();
    }
}