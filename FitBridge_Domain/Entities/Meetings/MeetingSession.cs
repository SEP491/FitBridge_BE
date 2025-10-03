using System;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Trainings;

namespace FitBridge_Domain.Entities.Meetings;

public class MeetingSession : BaseEntity
{
    public Guid UserOneId { get; set; }
    public Guid UserTwoId { get; set; }
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; }
    public ApplicationUser UserOne { get; set; }
    public ApplicationUser UserTwo { get; set; }
}
