using System;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Domain.Entities.Trainings;

public class Booking : BaseEntity
{
    public DateOnly BookingDate { get; set; }
    public TimeOnly? PtFreelanceStartTime { get; set; }
    public TimeOnly? PtFreelanceEndTime { get; set; }
    public Guid? PTGymSlotId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? OrderId { get; set; }
    public SessionStatus SessionStatus { get; set; }
    public string? Note { get; set; }
    public string? NutritionTip { get; set; }
    public PTGymSlot? PTGymSlot { get; set; }
    public ApplicationUser Customer { get; set; }
    public Order? Order { get; set; }
    public ICollection<SessionActivity> SessionActivities { get; set; } = new List<SessionActivity>();
}

public enum SessionStatus
{
    Cancelled,
    Booked,
    Finished
}
