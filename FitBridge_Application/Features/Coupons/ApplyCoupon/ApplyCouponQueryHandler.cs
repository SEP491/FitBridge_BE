using FitBridge_Application.Dtos.Coupons;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Coupons.GetCouponToApply;
using FitBridge_Application.Specifications.Orders.GetOrderByCouponAndUserId;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Coupons.ApplyCoupon
{
    internal class ApplyCouponQueryHandler(
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IUserUtil userUtil) : IRequestHandler<ApplyCouponQuery, ApplyCouponDto>
    {
        async Task<ApplyCouponDto> IRequestHandler<ApplyCouponQuery, ApplyCouponDto>.Handle(ApplyCouponQuery request, CancellationToken cancellationToken)
        {
            var accountId = userUtil.GetAccountId(httpContextAccessor.HttpContext) ?? throw new NotFoundException(nameof(ApplicationUser));
            var couponSpec = new GetCouponToApplySpecification(request.CouponCode);
            var coupon = await unitOfWork.Repository<Coupon>().GetBySpecificationAsync(couponSpec)
                ?? throw new NotFoundException(nameof(Coupon));

            var orderSpec = new GetOrderByCouponAndUserIdSpecification(coupon.Id, accountId);
            var order = await unitOfWork.Repository<Order>().GetBySpecificationAsync(orderSpec);

            if (coupon.Type.Equals(CouponType.FreelancePT) && order != null)
            {
                throw new InvalidDataException("PT coupon can only be used once");
            }

            if (coupon.Quantity - 1 <= 0)
            {
                throw new OutOfStocksException(nameof(Coupon));
            }

            var dto = new ApplyCouponDto
            {
                Id = coupon.Id,
                DiscountAmount = CalculateTotalPrice(),
                DiscountPercent = coupon.DiscountPercent,
            };

            return dto;
            decimal CalculateTotalPrice()
            {
                var discountAmount = Math.Max(request.TotalPrice * (decimal)coupon.DiscountPercent / 100, coupon.MaxDiscount);
                return request.TotalPrice - discountAmount;
            }
        }
    }
}