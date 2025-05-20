namespace TagStudio.Identity.Features;

/// <summary>
/// Represents JWT token and related data to be sent to user after login
/// </summary>
/// <param name="AccessToken">JWT access token</param>
/// <param name="RefreshToken">Refresh token</param>
/// <param name="ExpiresAtUtc">The expiration time of the access token as a UTC Unixtimestamp</param>
public record TokenData(string AccessToken, string RefreshToken, long ExpiresAtUtc);