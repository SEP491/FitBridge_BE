using System;

namespace FitBridge_Application.Dtos.Identities;

public class LoginResponseDTO
{
    public string IdToken { get; set; }

    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public string TokenType { get; set; } = "Bearer";
}

