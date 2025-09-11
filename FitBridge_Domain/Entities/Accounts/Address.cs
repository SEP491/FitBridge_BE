using System;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Domain.Entities.Accounts;

public class Address : BaseEntity
{
    public string ReceiverName { get; set; }
    public string PhoneNumber { get; set; }
    public string AddressDetail { get; set; }
    public string Note { get; set; }
    public Guid CustomerId { get; set; }
    public ApplicationUser Customer { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
