using System;
using FitBridge_Application.Dtos.OrderItems;
using FitBridge_Domain.Enums.Gyms;

namespace FitBridge_Application.Dtos.Gym;

public class GetGymOwnerCustomerDetail
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public Guid? LatestCustomerPurchasedId { get; set; }
    public string? Gender { get; set; }
    public string? PackageName { get; set; }
    public string? PtName { get; set; }
    public string? PtAvatarUrl { get; set; }
    public string AvatarUrl { get; set; }
    public DateOnly ExpirationDate { get; set; }
    public GymOwnerCustomerStatus Status { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? Dob { get; set; }
    public int PtGymAvailableSession { get; set; }
    public List<OrderItemForGymOwnerCust>? OrderItemsList { get; set; }
}
