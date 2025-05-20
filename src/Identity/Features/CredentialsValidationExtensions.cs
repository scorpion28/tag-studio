using FluentValidation;

namespace TagStudio.Identity.Features;

public static class CredentialsValidationExtensions
{
    public static IRuleBuilderOptions<T, string> ValidEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Email cannot be empty")
            .Matches(@"^[^@\s]+@[^@\s]+\.[\S]+$")
            .WithMessage("Email is invalid")
            .WithErrorCode("ValidEmailValidator");
    }

    public static IRuleBuilderOptions<T, string> ValidPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Password cannot be empty")
            .MinimumLength(8)
            .MaximumLength(40)
            .Matches(@"^(?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]).{8,40}$")
            .WithMessage("Password must be a mix of lowercase and uppercase letters, numbers and special characters")
            .WithErrorCode("StrongPasswordValidator");
    }
}