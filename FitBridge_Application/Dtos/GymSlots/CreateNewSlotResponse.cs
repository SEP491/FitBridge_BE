using System;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Dtos.GymSlots;

public class CreateNewSlotResponse
{
    [JsonIgnore]
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
}