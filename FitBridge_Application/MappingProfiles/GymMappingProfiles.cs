using AutoMapper;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.MappingProfiles
{
    public class GymMappingProfiles : Profile
    {
        public GymMappingProfiles()
        {
            CreateProjection<ApplicationUser, GetGymDetailsDto>();
        }
    }
}