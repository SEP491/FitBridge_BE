using AutoMapper;
using FitBridge_Application.Dtos.Coupons;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.MappingProfiles
{
    public class CouponMappingProfile : Profile
    {
        public CouponMappingProfile()
        {
            CreateMap<Coupon, CreateNewCouponDto>();
            CreateProjection<Coupon, GetCouponsDto>();
        }
    }
}