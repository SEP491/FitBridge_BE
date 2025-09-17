using System;

namespace FitBridge_Application.Dtos.Identities;

public class CreateNewPTResponse
{
        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? FullName { get; set; }

        public DateOnly? Dob { get; set; }

        public double? Weight { get; set; }

        public double? Height { get; set; }

        public List<string>? GoalTrainings { get; set; }

        public int? Experience { get; set; }

        public string? Gender { get; set; }
}
