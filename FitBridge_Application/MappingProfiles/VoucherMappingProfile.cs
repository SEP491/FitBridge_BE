using AutoMapper;
using FitBridge_Application.Dtos.Coupons;
using FitBridge_Application.Features.Vouchers.CreateFreelancePTVoucher;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.MappingProfiles
{
    public class VoucherMappingProfile : Profile
    {
        public VoucherMappingProfile()
        {
            CreateMap<Coupon, CreateNewCouponDto>();
            CreateProjection<Coupon, GetCouponsDto>();
        }
    }
}