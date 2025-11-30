using System;
using FitBridge_Application.Dtos.GymPTs;

namespace FitBridge_Application.Dtos.GymCourses;

public class GymCourseResponse
{
    public Guid? Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal PtPrice { get; set; }
    public Guid? GymPtId { get; set; }
    public int Session { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public Guid? GymOwnerId { get; set; }
    public GymPtResponseDto? Pt { get; set; }
}
