using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Entities.Meetings;
using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Domain.Entities.Trainings;

public class Booking : BaseEntity
{
    public DateOnly BookingDate { get; set; }

    public TimeOnly? PtFreelanceStartTime { get; set; }

    public TimeOnly? PtFreelanceEndTime { get; set; }

    public Guid? PTGymSlotId { get; set; }

    public Guid CustomerId { get; set; }

    public Guid CustomerPurchasedId { get; set; }

    public SessionStatus SessionStatus { get; set; }

    public string? Note { get; set; }

    public string? NutritionTip { get; set; }

    public PTGymSlot? PTGymSlot { get; set; }

    public ApplicationUser Customer { get; set; }

    public CustomerPurchased CustomerPurchased { get; set; }

    public ICollection<SessionActivity> SessionActivities { get; set; } = new List<SessionActivity>();
    public MeetingSession? MeetingSession { get; set; }
}