﻿namespace TagStudio.WebApi.Features.Users.Refresh;

public class RefreshRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}