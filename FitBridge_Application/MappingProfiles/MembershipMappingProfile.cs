using AutoMapper;
using FitBridge_Application.Dtos.Membership;
using FitBridge_Domain.Entities.ServicePackages;

namespace FitBridge_Application.MappingProfiles
{
    public class MembershipMappingProfile : Profile
    {
        public MembershipMappingProfile()
        {
            CreateMap<ServiceInformation, CreateMembershipDto>();
            CreateMap<ServiceInformation, GetMembershipDto>();
            CreateMap<ServiceInformation, UpdateMembershipDto>();
        }
    }
}