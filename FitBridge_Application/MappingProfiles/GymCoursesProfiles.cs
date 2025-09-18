using System;
using AutoMapper;
using FitBridge_Application.Features.GymCourses.Commands;
using FitBridge_Application.Dtos.GymCourses.Response;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.MappingProfiles;

public class GymCoursesProfiles : Profile
{
    public GymCoursesProfiles()
    {
        CreateMap<CreateGymCourseCommand, GymCourse>();
        CreateMap<GymCourse, CreateGymCourseResponse>();
    }
}
