using System;

namespace FitBridge_Application.Dtos.GymCourses.Response;

public class CreateGymCourseResponse
{
    public string? Name { get; set; }

    public double Price { get; set; }

    public long Duration { get; set; }

    public string? Type { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }
    public Guid GymOwnerId { get; set; }

}
