using FitBridge_Domain.Enums.Trainings;
using FitBridge_Domain.Enums.SessionActivities;
using FitBridge_Domain.Enums.ActivitySets;

namespace FitBridge_Domain.Entities.Trainings;

public class SessionActivity : BaseEntity
{
    public ActivityType ActivityType { get; set; }

    public string ActivityName { get; set; }
    public MuscleGroupEnum[] MuscleGroups { get; set; } = Array.Empty<MuscleGroupEnum>();
    public ActivitySetType ActivitySetType { get; set; }
    public Guid BookingId { get; set; }

    public Booking Booking { get; set; }

    public ICollection<ActivitySet> ActivitySets { get; set; } = new List<ActivitySet>();
}