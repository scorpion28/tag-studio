namespace TagStudio.Identity.Features;

public record LoginResponse(TokenData Jwt, UserInfo User);

public record UserInfo(string Name);