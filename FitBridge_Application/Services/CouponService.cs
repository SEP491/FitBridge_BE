using System;
using FitBridge_Application.Dtos.Coupons;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Orders.GetOrderByCouponAndUserId;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Services;

public class CouponService(IUnitOfWork unitOfWork)
{
    public async Task<ApplyCouponDto> ApplyCouponAsync(Guid userId, Guid couponId, decimal totalPrice)
    {
            var coupon = await unitOfWork.Repository<Coupon>().GetByIdAsync(couponId)
                ?? throw new NotFoundException(nameof(Coupon));

            var orderSpec = new GetOrderByCouponAndUserIdSpecification(coupon.Id, userId);
            var order = await unitOfWork.Repository<Order>().GetBySpecificationAsync(orderSpec);

            if (order != null)
            {
                throw new InvalidDataException("Coupon already applied");
            }

            if (coupon.Quantity - 1 <= 0)
            {
                throw new OutOfStocksException(nameof(Coupon));
            }

            if (coupon.ExpirationDate < DateOnly.FromDateTime(DateTime.UtcNow) || !coupon.IsActive)
            {
                throw new ExpiredException(coupon.CouponCode);
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
                var discountAmount = Math.Max(totalPrice * (decimal)coupon.DiscountPercent / 100, coupon.MaxDiscount);
                return totalPrice - discountAmount;
            }
    }
}
