using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Domain.Entities.Orders;

public class Coupon : BaseEntity
{
    public decimal MaxDiscount { get; set; }

    public CouponType Type { get; set; }

    public double DiscountPercent { get; set; }

    public int Quantity { get; set; }

    public Guid CreatorId { get; set; }
    public bool IsActive { get; set; }
    public string CouponCode { get; set; }

    public ApplicationUser Creator { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}