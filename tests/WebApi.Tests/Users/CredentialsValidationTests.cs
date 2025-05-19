using FluentValidation.TestHelper;
using Shouldly;
using WebApi.Tests.TestData;

namespace WebApi.Tests.Users;

public class CredentialsValidationTests
{
    private readonly TestCredentialsValidator _validator = new();

    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData(" ")]
    public void EmailValidation_ShouldFail_WhenEmptyEmail(string? email)
    {
        var credentials = new TestCredentials { Email = email!, Password = "StrongPassword@1" };

        var result = _validator.TestValidate(credentials);

        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(req => req.Email)
            .WithErrorCode("NotEmptyValidator");
    }

    [Theory]
    [MemberData(nameof(CredentialsTestData.InvalidEmails), MemberType = typeof(CredentialsTestData))]
    public void EmailValidation_ShouldFail_WhenInvalidEmail(string email)
    {
        var credentials = new TestCredentials { Email = email, Password = "StrongPassword@1" };

        var result = _validator.TestValidate(credentials);

        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(req => req.Email)
            .WithErrorCode("ValidEmailValidator");
    }

    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData(" ")]
    public void PasswordValidation_ShouldFail_WhenEmptyPassword(string? password)
    {
        var credentials = new TestCredentials { Email = "test@example.com", Password = password! };

        var result = _validator.TestValidate(credentials);

        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(req => req.Password)
            .WithErrorCode("NotEmptyValidator");
    }

    [Theory]
    [InlineData("pass", "MinimumLengthValidator")]
    [InlineData("abcdefg", "MinimumLengthValidator")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "MaximumLengthValidator")]
    public void PasswordValidation_ShouldFail_WhenPasswordLengthOutOfBounds(string? password, string errorCode)
    {
        var credentials = new TestCredentials { Email = "test@example.com", Password = password! };

        var result = _validator.TestValidate(credentials);

        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(req => req.Password)
            .WithErrorCode(errorCode);
    }

    [Theory]
    [InlineData("passwordA")]
    [MemberData(nameof(CredentialsTestData.WeakPasswords), MemberType = typeof(CredentialsTestData))]
    public void PasswordValidation_ShouldFail_WhenWeakPassword(string password)
    {
        var credentials = new TestCredentials { Email = "test@example.com", Password = password };

        var result = _validator.TestValidate(credentials);

        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(req => req.Password)
            .WithErrorCode("StrongPasswordValidator");
    }
}