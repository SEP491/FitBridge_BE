namespace FitBridge_Application.Dtos.Gym
{
    public class GetGymDetailsDto
    {
        public Guid Id { get; set; }

        public string GymName { get; set; } = string.Empty;

        public string RepresentName { get; set; } = string.Empty;

        public string GymAddress { get; set; } = string.Empty;

        public List<GymImageDto> GymImages { get; set; } = [];

        public double Longitude { get; set; } = 0.0;

        public double Latitude { get; set; } = 0.0;

        public bool HotResearch { get; set; } = false;

        public string GymDescription { get; set; } = string.Empty;
    }
}