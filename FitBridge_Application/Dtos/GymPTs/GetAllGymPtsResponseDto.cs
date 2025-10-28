namespace FitBridge_Application.Dtos.GymPTs
{
    public class GetAllGymPtsResponseDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public bool IsMale { get; set; }

        public DateTime Dob { get; set; }

        public string? AvatarUrl { get; set; }

        public string? Bio { get; set; }

        public int? Experience { get; set; }

        public List<string> GoalTrainings { get; set; } = new List<string>();

        public double Rating { get; set; }

        public int TotalCoursesAssigned { get; set; }

        public Guid? GymOwnerId { get; set; }

        public string? GymName { get; set; }
    }
}