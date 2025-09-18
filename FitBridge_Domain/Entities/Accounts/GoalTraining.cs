using System;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Domain.Entities.Accounts;

public class GoalTraining : BaseEntity
{
    public string Name { get; set; }
    public Guid GymOwnerId { get; set; }
    public ApplicationUser GymOwner { get; set; }
    public ICollection<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();
}
