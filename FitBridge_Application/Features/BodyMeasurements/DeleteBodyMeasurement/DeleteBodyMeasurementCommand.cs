using System;
using MediatR;

namespace FitBridge_Application.Features.BodyMeasurements.DeleteBodyMeasurement;

public class DeleteBodyMeasurementCommand : IRequest<bool>      
{
    public Guid Id { get; set; }
}
