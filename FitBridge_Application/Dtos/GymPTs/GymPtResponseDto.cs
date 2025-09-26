using System;

namespace FitBridge_Application.Dtos.GymPTs;

public class GymPtResponseDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public bool IsMale { get; set; }
    public DateTime Dob { get; set; }
    public string? AvatarUrl { get; set; }
    public int? Experience { get; set; }
}
