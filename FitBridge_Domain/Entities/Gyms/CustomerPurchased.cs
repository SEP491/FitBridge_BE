using System;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Entities.Trainings;

namespace FitBridge_Domain.Entities.Gyms;

public class CustomerPurchased : BaseEntity
{
    public int AvailableSessions { get; set; }
    public Guid CustomerId { get; set; }
    public DateOnly ExpirationDate { get; set; }
    public ApplicationUser Customer { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public UserGoal? UserGoal { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Order> OrderThatExtend { get; set; } = new List<Order>();
}
