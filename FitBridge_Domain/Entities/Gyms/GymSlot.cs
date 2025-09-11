using System;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Domain.Entities.Gyms;

public class GymSlot : BaseEntity
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Name { get; set; }
    public Guid GymOwnerId { get; set; }
    public ApplicationUser GymOwner { get; set; }
    public ICollection<PTGymSlot> PTGymSlots { get; set; } = new List<PTGymSlot>();
}
