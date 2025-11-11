using System;

namespace FitBridge_Application.Dtos.Weights;

public class WeightResponseDto
{
    public Guid Id { get; set; }
    public double Value { get; set; }
    public string Unit { get; set; }
}
