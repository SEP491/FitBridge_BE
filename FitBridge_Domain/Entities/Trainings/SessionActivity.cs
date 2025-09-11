using System;
using FitBridge_Domain.Entities;

namespace FitBridge_Domain.Entities.Trainings;

public class SessionActivity : BaseEntity
{
    public ActivityType ActivityType { get; set; }
    public string ActivityName { get; set; }
    public List<string> MuscleGroups { get; set; } = new List<string>();
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; }
    public ICollection<ActivitySet> ActivitySets { get; set; } = new List<ActivitySet>();
}

public enum ActivityType //Loại bài tập ví dụ như là workout, tập với thiết bị, khởi động
{
    WarmUp,
    Workout,
    WithEquiment
}
