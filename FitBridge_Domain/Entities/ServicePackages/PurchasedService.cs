using System;

namespace FitBridge_Domain.Entities.ServicePackages;

public class PurchasedService : BaseEntity
{
    public decimal ServiceCharge { get; set; }
    public Guid AccountId { get; set; }
    public DateTime ExpirationDate { get; set; }
    public Guid OrderItemId { get; set; }
}
