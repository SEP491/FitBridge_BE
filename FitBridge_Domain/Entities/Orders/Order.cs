using System;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.ServicePackages;
using FitBridge_Domain.Entities.Trainings;

namespace FitBridge_Domain.Entities.Orders;

public class Order : BaseEntity
{
    public OrderStatus Status { get; set; }
    public string CheckoutUrl { get; set; }
    public decimal TotalAmount { get; set; }
    public int AvailableSessions { get; set; }
    public Guid? GymCoursePTId { get; set; }
    public GymCoursePT? GymCoursePT { get; set; }
    public Guid AddressId { get; set; }
    public Address Address { get; set; }
    public Guid AccountId { get; set; }
    public ApplicationUser Account { get; set; }
    public Guid? PTFreelancePackageId { get; set; }
    public FreelancePTPackage? PTFreelancePackage { get; set; }
    public Guid? VoucherId { get; set; }
    public Voucher? Voucher { get; set; }
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public Guid? ServiceInformationId { get; set; }
    public ServiceInformation? ServiceInformation { get; set; }
    public UserGoal? UserGoal { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public Booking? Booking { get; set; }
}

public enum OrderStatus
{
    PaymentProcessing,
    Pending,
    Processing,
    Arrrived,
    Cancelled
}
