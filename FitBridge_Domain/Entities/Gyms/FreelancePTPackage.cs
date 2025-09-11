using System;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Domain.Entities.Gyms;

public class FreelancePTPackage : BaseEntity
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int DurationInDays { get; set; }
    public int NumOfSessions { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public Guid PtId { get; set; }
    public ApplicationUser Pt { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
