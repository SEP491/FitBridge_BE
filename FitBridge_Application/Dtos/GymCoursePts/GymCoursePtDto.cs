using System;

namespace FitBridge_Application.Dtos.GymCoursePts;

public class GymCoursePtDto
{
    public Guid Id { get; set; }
    public Guid GymCourseId { get; set; }
    public Guid PTId { get; set; }
    public int? Session { get; set; }
    public string PtImageUrl { get; set; } = string.Empty;
    public string PtName { get; set; } = string.Empty;
}
