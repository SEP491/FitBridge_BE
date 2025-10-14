using System;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Domain.Entities.Trainings;

public class UserGoal : BaseEntity
{
    public double? TargetBiceps { get; set; }
    public double? TargetForeArm { get; set; }
    public double? TargetThigh { get; set; }
    public double? TargetCalf { get; set; }
    public double? TargetChest { get; set; }
    public double? TargetWaist { get; set; }
    public double? TargetHip { get; set; }
    public double? TargetShoulder { get; set; }
    public double? TargetHeight { get; set; }
    public double? TargetWeight { get; set; }

    public double? StartBiceps { get; set; }
    public double? StartForeArm { get; set; }
    public double? StartThigh { get; set; }
    public double? StartCalf { get; set; }
    public double? StartChest { get; set; }
    public double? StartWaist { get; set; }
    public double? StartHip { get; set; }
    public double? StartShoulder { get; set; }
    public double? StartHeight { get; set; }
    public double? StartWeight { get; set; }

    public double? FinalBiceps { get; set; }
    public double? FinalForeArm { get; set; }
    public double? FinalThigh { get; set; }
    public double? FinalCalf { get; set; }
    public double? FinalChest { get; set; }
    public double? FinalWaist { get; set; }
    public double? FinalHip { get; set; }
    public double? FinalShoulder { get; set; }

    public string? ImageUrl { get; set; }
    public string? FinalImageUrl { get; set; }
    public Guid CustomerPurchasedId { get; set; }
    public CustomerPurchased CustomerPurchased { get; set; }

}
