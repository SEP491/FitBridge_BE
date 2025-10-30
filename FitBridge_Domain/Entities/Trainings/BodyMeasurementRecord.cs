using System;
using FitBridge_Domain.Entities.Gyms;
namespace FitBridge_Domain.Entities.Trainings;

public class BodyMeasurementRecord : BaseEntity
{
    public double? Biceps { get; set; }
    public double? ForeArm { get; set; }
    public double? Thigh { get; set; }
    public double? Calf { get; set; }
    public double? Chest { get; set; }
    public double? Waist { get; set; }
    public double? Hip { get; set; }
    public double? Shoulder { get; set; }
    public double? Height { get; set; }
    public double? Weight { get; set; }
    public Guid CustomerPurchasedId { get; set; }
    public CustomerPurchased CustomerPurchased { get; set; }
}
