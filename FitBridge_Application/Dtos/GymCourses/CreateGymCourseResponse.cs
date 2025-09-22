using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Enums.GymCourses;
using System;

namespace FitBridge_Application.Dtos.GymCourses;

public class CreateGymCourseResponse
{
    public string? Name { get; set; }

    public decimal Price { get; set; }

    public long Duration { get; set; }

    public TypeCourseEnum Type { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public Guid GymOwnerId { get; set; }
}