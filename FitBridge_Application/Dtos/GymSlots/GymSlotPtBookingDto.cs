using System;
using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Application.Dtos.GymSlots;

public class GymSlotPtBookingDto
{
    public GymPtRegisterSlot PtGymSlot { get; set; }
    public string CustomerName { get; set; }
    public string CustomerAvatarUrl { get; set; }
    public string CustomerId { get; set; }
    public SessionStatus SessionStatus { get; set; }
}
