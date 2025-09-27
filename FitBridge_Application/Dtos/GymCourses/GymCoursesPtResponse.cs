using System;
using FitBridge_Domain.Enums.GymCourses;
using FitBridge_Application.Dtos.GymPTs;

namespace FitBridge_Application.Dtos.GymCourses;

public class GymCoursesPtResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public decimal Price { get; set; }

    public Guid GymPtId { get; set; }
    public DateOnly ExpirationDate { get; set; }
    public int AvailableSessions { get; set; }

    public TypeCourseEnum Type { get; set; }

    public string Description { get; set; }

    public string ImageUrl { get; set; }

    public Guid GymOwnerId { get; set; }
    public GymPtResponseDto GymPt { get; set; }
}
