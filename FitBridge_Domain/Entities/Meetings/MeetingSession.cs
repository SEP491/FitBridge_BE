using System;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Domain.Entities.Meetings;

public class MeetingSession : BaseEntity
{
    public DateTime StartTime { get; set; }
    public Guid UserOneId { get; set; }
    public Guid UserTwoId { get; set; }
    public ApplicationUser UserOne { get; set; }
    public ApplicationUser UserTwo { get; set; }
}
