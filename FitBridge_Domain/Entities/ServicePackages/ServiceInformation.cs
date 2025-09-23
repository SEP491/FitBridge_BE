using System;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Domain.Entities.ServicePackages;

public class ServiceInformation : BaseEntity
{
    public string ServiceName { get; set; }
    public decimal ServiceCharge { get; set; }
    public int? MaximumHotResearchSlot { get; set; }
    public int? AvailableHotResearchSlot { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
