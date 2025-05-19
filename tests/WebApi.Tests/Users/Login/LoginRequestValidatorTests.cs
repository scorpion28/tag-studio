using FluentValidation.TestHelper;
using Shouldly;
using TagStudio.WebApi.Features.Users;
using WebApi.Tests.TestData;

namespace WebApi.Tests.Users;

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator = new();

    [Theory]
    [MemberData(nameof(CredentialsTestData.ValidCredentials), MemberType = typeof(CredentialsTestData))]
    public void Validation_ShouldSucceed_WhenValidRequestData(string email, string password)
    {
        var request = new LoginRequest { Email = email, Password = password };

        var isValid = _validator.TestValidate(request).IsValid;

        isValid.ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(CredentialsTestData.InvalidCredentials), MemberType = typeof(CredentialsTestData))]
    public void Validation_ShouldFail_WhenInvalidRequestData(string email, string password)
    {
        var request = new LoginRequest { Email = email, Password = password };

        var result = _validator.TestValidate(request);

        result.IsValid.ShouldBeFalse();
    }
}