using System;

namespace FitBridge_Application.Dtos.Gym;

public class GetAllGymPtsForAdminResponseDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public bool IsMale { get; set; }
    public DateTime? Dob { get; set; }
    public string? AvatarUrl { get; set; }
    public int Experience { get; set; }
    public Guid? GymOwnerId { get; set; }
    public string? GymOwnerName { get; set; }
    public bool IsActive { get; set; }
}
