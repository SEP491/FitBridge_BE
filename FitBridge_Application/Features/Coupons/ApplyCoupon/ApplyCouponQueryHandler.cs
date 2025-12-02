using FitBridge_Application.Dtos.Coupons;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Services;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Coupons.ApplyCoupon
{
    internal class ApplyCouponQueryHandler(
        CouponService couponService,
        IHttpContextAccessor httpContextAccessor,
        IUserUtil userUtil) : IRequestHandler<ApplyCouponQuery, ApplyCouponDto>
    {
        async Task<ApplyCouponDto> IRequestHandler<ApplyCouponQuery, ApplyCouponDto>.Handle(
            ApplyCouponQuery request, 
            CancellationToken cancellationToken)
        {
            var accountId = userUtil.GetAccountId(httpContextAccessor.HttpContext) 
                ?? throw new NotFoundException(nameof(ApplicationUser));

            return await couponService.ApplyCouponWithValidationAsync(
                request.CouponCode,
                accountId,
                request.ItemsId,
                request.ProductType,
                request.TotalPrice
            );
        }
    }
}