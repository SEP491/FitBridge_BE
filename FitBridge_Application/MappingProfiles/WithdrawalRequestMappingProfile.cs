using AutoMapper;
using FitBridge_Application.Dtos.Payments;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.MappingProfiles
{
    public class WithdrawalRequestMappingProfile : Profile
    {
        public WithdrawalRequestMappingProfile()
        {
            CreateProjection<WithdrawalRequest, GetWithdrawalRequestResponseDto>()
                .ForMember(dest => dest.AccountFullName, opt => opt.MapFrom(src => src.Account.FullName));
        }
    }
}
