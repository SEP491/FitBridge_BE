using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Enums.Orders;

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