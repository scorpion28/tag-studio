namespace TagStudio.Identity.Features;

public class SignupRequest
{
    public string Email { get; init; } = null!;

    public string Password { get; init; } = null!;
}