using System;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Domain.Entities.Orders;

public class Voucher : BaseEntity
{
    public decimal MaxDiscount { get; set; }
    public VoucherType Type { get; set; }
    public double DiscountPercent { get; set; }
    public int Quantity { get; set; }
    public Guid CreatorId { get; set; }
    public ApplicationUser Creator { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();

}

public enum VoucherType
{
    FreelancePT,
    System
}
