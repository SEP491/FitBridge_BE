using System;
using FitBridge_Application.Dtos.Accounts.UserDetails;

namespace FitBridge_Application.Dtos.Gym;

public class GetGymPtsDetailForAdminResponseDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public bool IsMale { get; set; }
    public DateTime? Dob { get; set; }
    public string? AvatarUrl { get; set; }
    public List<string> GoalTrainings { get; set; } = new List<string>();
    public Guid? GymOwnerId { get; set; }
    public string? GymName { get; set; }
    public UserDetailDto? UserDetail { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int PtMaxCourse { get; set; }
}
