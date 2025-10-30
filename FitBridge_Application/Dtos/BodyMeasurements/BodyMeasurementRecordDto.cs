using System;

namespace FitBridge_Application.Dtos.BodyMeasurements;

public class BodyMeasurementRecordDto
{
    public Guid Id { get; set; }
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
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
