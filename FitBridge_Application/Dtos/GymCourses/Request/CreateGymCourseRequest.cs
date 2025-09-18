using System;
using FitBridge_Domain.Entities.Gyms;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Dtos.GymCourses.Request;

public class CreateGymCourseRequest
{
    [JsonIgnore]
    public Guid? GymOwnerId { get; set; }
    public string Name { get; set; } = null!;

    public double Price { get; set; }

    public long Duration { get; set; }

    public TypeCourseEnum Type { get; set; }

    public string Description { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;
}
