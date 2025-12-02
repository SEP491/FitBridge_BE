using System;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Domain.Entities.Accounts;

public class Address : BaseEntity
{
    public string ReceiverName { get; set; }
    public string PhoneNumber { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public string Ward { get; set; }
    public string Street { get; set; }
    public string HouseNumber { get; set; }
    public string Note { get; set; }
    public Guid CustomerId { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? GoogleMapAddressString { get; set; }
    public bool IsShopDefaultAddress { get; set; }
    public ApplicationUser Customer { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
