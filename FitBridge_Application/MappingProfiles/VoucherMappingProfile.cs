using AutoMapper;
using FitBridge_Application.Dtos.Vouchers;
using FitBridge_Application.Features.Vouchers.CreateFreelancePTVoucher;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.MappingProfiles
{
    public class VoucherMappingProfile : Profile
    {
        public VoucherMappingProfile()
        {
            CreateMap<Voucher, CreateNewVoucherDto>();
            CreateProjection<Voucher, GetVouchersDto>();
        }
    }
}