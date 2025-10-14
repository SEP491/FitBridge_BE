using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Application.Dtos.SessionActivities
{
    public class GetSessionActivitiesResponseDto
    {
        public Guid Id { get; set; }

        public ActivityType ActivityType { get; set; }

        public string ActivityName { get; set; }
    }
}