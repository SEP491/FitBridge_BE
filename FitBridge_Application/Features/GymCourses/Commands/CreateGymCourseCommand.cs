using System;
using FitBridge_Application.Dtos.GymCourses.Request;
using FitBridge_Application.Dtos.GymCourses.Response;
using FitBridge_Domain.Entities.Gyms;
using MediatR;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.GymCourses.Commands;

public class CreateGymCourseCommand : IRequest<CreateGymCourseResponse>
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
