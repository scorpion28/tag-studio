using TagStudio.WebApi.Features.Authentication;

namespace TagStudio.WebApi.Features.Users;

public record LoginResponse(TokenData Jwt, UserInfo User);

public record UserInfo(string Name);