using AutoMapper;
using FitBridge_Application.Features.GymCourses.AssignPtToCourse;
using FitBridge_Application.Features.GymCourses.CreateGymCourse;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.MappingProfiles
{
    public class CourseMappingProfile : Profile
    {
        public CourseMappingProfile()
        {
            CreateMap<CreateGymCourseCommand, GymCourse>();
            CreateMap<AssignPtToCourseCommand, GymCoursePT>();
        }
    }
}