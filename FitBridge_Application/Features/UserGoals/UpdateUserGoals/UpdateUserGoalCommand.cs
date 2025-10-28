using System;
using FitBridge_Application.Dtos.UserGoals;
using MediatR;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.UserGoals.UpdateUserGoals;

public class UpdateUserGoalCommand : IRequest<UserGoalsDto>
{
    [JsonIgnore]
    public Guid CustomerPurchasedId { get; set; }
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

    public double? CurrentBiceps { get; set; }
    public double? CurrentForeArm { get; set; }
    public double? CurrentThigh { get; set; }
    public double? CurrentCalf { get; set; }
    public double? CurrentChest { get; set; }
    public double? CurrentWaist { get; set; }
    public double? CurrentHip { get; set; }
    public double? CurrentShoulder { get; set; }
    public double? CurrentHeight { get; set; }
    public double? CurrentWeight { get; set; }

    public string? ImageUrl { get; set; }
    public string? FinalImageUrl { get; set; }
}
