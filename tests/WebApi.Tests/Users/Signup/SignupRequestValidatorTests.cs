using FluentValidation.TestHelper;
using Shouldly;
using TagStudio.Identity.Features;
using WebApi.Tests.TestData;

namespace WebApi.Tests.Users;

public class SignupRequestValidatorTests
{
    private readonly SignupRequestValidator _validator = new();

    [Theory]
    [MemberData(nameof(CredentialsTestData.ValidCredentials), MemberType = typeof(CredentialsTestData))]
    public void Validation_ShouldSucceed_WhenValidRequestData(string email, string password)
    {
        var request = new SignupRequest { Email = email, Password = password };

        var isValid = _validator.TestValidate(request).IsValid;

        isValid.ShouldBeTrue();
    }

    [Fact]
    public void Validation_ShouldFail_WhenInvalidRequestData()
    {
        var request = new SignupRequest { Email = "", Password = "" };

        var result = _validator.TestValidate(request);

        result.IsValid.ShouldBeFalse();
    }
}