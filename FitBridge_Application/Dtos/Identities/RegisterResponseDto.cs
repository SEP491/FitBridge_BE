using System;

namespace FitBridge_Application.Dtos.Identities;

public class RegisterResponseDto
{
    public string Status { get; set; }
    public string Message { get; set; }
    public Guid UserId { get; set; }
}
