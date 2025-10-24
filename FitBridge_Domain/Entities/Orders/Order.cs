using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.ServicePackages;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Domain.Entities.Orders;

public class Order : BaseEntity
{
    public OrderStatus Status { get; set; }
    public string CheckoutUrl { get; set; }
    public decimal SubTotalPrice { get; set; }
    public Guid? AddressId { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal TotalAmount { get; set; }
    public Guid? CustomerPurchasedIdToExtend { get; set; }
    public Guid? GymCoursePTIdToAssign { get; set; }
    public CustomerPurchased? CustomerPurchasedToExtend { get; set; }

    public Address Address { get; set; }

    public Guid AccountId { get; set; }

    public ApplicationUser Account { get; set; }

    public Guid? CouponId { get; set; }

    public Coupon? Coupon { get; set; }
    public GymCoursePT? GymCoursePTToAssign { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}