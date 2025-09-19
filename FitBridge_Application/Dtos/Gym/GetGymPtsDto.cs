using System;

namespace FitBridge_Application.Dtos.Gym
{
    public class GetGymPtsDto
    {
        public string Id { get; set; } = default!;

        public string FullName { get; set; } = default!;

        public string Phone { get; set; } = default!;

        public string Email { get; set; } = default!;

        public DateOnly Dob { get; set; }

        public string GoalTraining { get; set; } = default!;

        public double Weight { get; set; }

        public double Height { get; set; }

        public int Experience { get; set; }

        public string Gender { get; set; } = default!;

        public string AvatarUrl { get; set; }
    }
}