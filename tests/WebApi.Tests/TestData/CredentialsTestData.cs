namespace WebApi.Tests.TestData;

public static class CredentialsTestData
{
    public static readonly TheoryData<string, string> ValidCredentials = new()
    {
        { "test@example.com", "@StrongPassword1" },
        { "test1@example.com", "!@#$%^&*(aA1" },
        { "test.email@example.com", "901@R(D[c£oR" },
        { "test+email@example.com", "5o5St097)N@.@u(HN" },
        { "test@subdomain.example.com", "lS0$,z+w#£thd86(2" }
    };

    public static readonly TheoryData<string, string> InvalidCredentials = new()
    {
        { "InvalidEmail", "ValidPassword1@" },
        { "validemail@example.com", "InvalidPassword" },
        { "InvalidEmail", "InvalidPassword" },
        { "test@", "pass" },
        { "", "" }
    };

    public static readonly TheoryData<string> InvalidEmails =
    [
        "email",
        "@example.com",
        "test @example.com",
        "test@",
        "test@.com",
        "test@example",
        "test@example_com",
        "test@example@com"
    ];

    public static readonly TheoryData<string> WeakPasswords =
    [
        "passwordA",
        "password1",
        "password@",
        "password@A",
        "password@1",
        "password1A",
        "йцукенгш"
    ];
}