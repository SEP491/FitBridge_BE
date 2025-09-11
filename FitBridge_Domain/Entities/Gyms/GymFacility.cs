using System;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Domain.Entities.Gyms;

public class GymFacility : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public List<string> ImageUrls { get; set; } = new List<string>();
    public Guid GymOwnerId { get; set; }
    public ApplicationUser GymOwner { get; set; }
}
