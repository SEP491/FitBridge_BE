using System;
using System.Text.Json.Serialization;
using FitBridge_Application.Dtos.BodyMeasurements;
using MediatR;

namespace FitBridge_Application.Features.BodyMeasurements.UpdateBodyMeasurement;

public class UpdateBodyMeasurementCommand : IRequest<BodyMeasurementRecordDto>
{
    [JsonIgnore]
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
}
