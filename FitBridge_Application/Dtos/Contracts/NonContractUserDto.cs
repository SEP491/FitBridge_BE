using System;

namespace FitBridge_Application.Dtos.Contracts;

public class NonContractUserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; }
}
