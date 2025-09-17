using System;

namespace FitBridge_Application.Dtos.Identities;

public class CreateNewGymPTRequest
{
    public string FullName { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public double Weight { get; set; }

    public double Height { get; set; }

    public List<string> GoalTrainings { get; set; } = null!;

    public int Experience { get; set; }

    public bool IsMale { get; set; }
}
