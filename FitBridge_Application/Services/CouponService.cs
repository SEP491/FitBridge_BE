using System;
using FitBridge_Application.Dtos.Coupons;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Orders.GetOrderByCouponAndUserId;
using FitBridge_Application.Specifications.Coupons.GetCouponToApply;
using FitBridge_Application.Specifications.GymCourses.GetGymCourseByIds;
using FitBridge_Application.Specifications.FreelancePtPackages.GetFreelancePTPackagesByIds;
using FitBridge_Application.Specifications.ProductDetails.GetProductDetailsByIds;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Services;

public class CouponService(IUnitOfWork unitOfWork)
{
    /// <summary>
    /// Validates and applies a coupon with full business rule validation
    /// </summary>
    public async Task<ApplyCouponDto> ApplyCouponWithValidationAsync(
        string couponCode, 
        Guid userId, 
        List<Guid> itemsId, 
        string productType, 
        decimal totalPrice)
    {
        // Get coupon by code
        var couponSpec = new GetCouponToApplySpecification(couponCode);
        var coupon = await unitOfWork.Repository<Coupon>().GetBySpecificationAsync(couponSpec)
            ?? throw new NotFoundException(nameof(Coupon));

        // Validate coupon stock
        if (coupon.Quantity - coupon.NumberOfUsedCoupon <= 0)
        {
            throw new OutOfStocksException(nameof(Coupon));
        }

        // Validate expiration and active status
        if (coupon.ExpirationDate < DateOnly.FromDateTime(DateTime.UtcNow) || !coupon.IsActive)
        {
            throw new ExpiredException(coupon.CouponCode);
        }

        // Check if user already used this coupon
        var orderSpec = new GetOrderByCouponAndUserIdSpecification(coupon.Id, userId);
        var existingOrder = await unitOfWork.Repository<Order>().GetBySpecificationAsync(orderSpec);

        if (existingOrder != null)
        {
            throw new CouponAlreadyAppliedException(coupon.CouponCode);
        }

        // Validate product similarity and get creator
        var productCreatorId = await ValidateProductSimilarityAsync(itemsId, productType);

        // Validate coupon type matches product type
        if (productType == nameof(Product) && coupon.Type != CouponType.System)
        {
            throw new DataValidationFailedException("Product orders can only use system coupons");
        }
        else if (productType != nameof(Product) && coupon.CreatorId != productCreatorId)
        {
            throw new DataValidationFailedException("This coupon can only be applied to products of the same creator");
        }

        // Calculate discount
        return new ApplyCouponDto
        {
            Id = coupon.Id,
            DiscountAmount = CalculateTotalPrice(totalPrice, coupon),
            DiscountPercent = coupon.DiscountPercent,
        };
    }

    /// <summary>
    /// Simple coupon application by ID (legacy method, used in CreatePaymentLink)
    /// </summary>
    public async Task<ApplyCouponDto> ApplyCouponAsync(Guid userId, Guid couponId, decimal totalPrice)
    {
        var coupon = await unitOfWork.Repository<Coupon>().GetByIdAsync(couponId)
            ?? throw new NotFoundException(nameof(Coupon));

        var orderSpec = new GetOrderByCouponAndUserIdSpecification(coupon.Id, userId);
        var order = await unitOfWork.Repository<Order>().GetBySpecificationAsync(orderSpec);

        if (order != null)
        {
            throw new BusinessException("Coupon already applied");
        }

        if (coupon.Quantity - 1 <= 0)
        {
            throw new OutOfStocksException(nameof(Coupon));
        }

        if (coupon.ExpirationDate < DateOnly.FromDateTime(DateTime.UtcNow) || !coupon.IsActive)
        {
            throw new ExpiredException(coupon.CouponCode);
        }

        return new ApplyCouponDto
        {
            Id = coupon.Id,
            DiscountAmount = CalculateTotalPrice(totalPrice, coupon),
            DiscountPercent = coupon.DiscountPercent,
        };
    }

    private static decimal CalculateTotalPrice(decimal totalPrice, Coupon coupon)
    {
        var discountAmount = Math.Min(totalPrice * (decimal)coupon.DiscountPercent / 100, coupon.MaxDiscount);
        return totalPrice - discountAmount;
    }

    private async Task<Guid?> ValidateProductSimilarityAsync(List<Guid> itemsId, string productType)
    {
        if (itemsId == null || itemsId.Count == 0)
        {
            throw new DataValidationFailedException("Cannot apply coupon to empty cart");
        }

        switch (productType)
        {
            case nameof(GymCourse):
                {
                    var gymCourse = await unitOfWork.Repository<GymCourse>()
                        .GetAllWithSpecificationAsync(new GetGymCourseByIdsSpec(itemsId));

                    if (gymCourse.Count != itemsId.Count)
                    {
                        throw new DataValidationFailedException("Some gym courses were not found");
                    }

                    var distinctCreatorIds = gymCourse.Select(x => x.GymOwnerId).Distinct();
                    if (distinctCreatorIds.Count() > 1)
                    {
                        throw new DataValidationFailedException("This coupon can only be applied to gym courses of the same creator");
                    }
                    return distinctCreatorIds.First();
                }

            case nameof(FreelancePTPackage):
                {
                    var packages = await unitOfWork.Repository<FreelancePTPackage>()
                        .GetAllWithSpecificationAsync(new GetFreelancePTPackagesByIdsSpec(itemsId));

                    if (packages.Count != itemsId.Count)
                    {
                        throw new DataValidationFailedException("Some freelance PT packages were not found");
                    }

                    var distinctCreatorIds = packages.Select(x => x.PtId).Distinct();
                    if (distinctCreatorIds.Count() > 1)
                    {
                        throw new DataValidationFailedException("This coupon can only be applied to freelance PT packages of the same creator");
                    }
                    return distinctCreatorIds.First();
                }

            case nameof(Product):
                var productDetailsCount = await unitOfWork.Repository<ProductDetail>()
                    .CountAsync(new GetProductDetailsByIdsSpec(itemsId));

                if (productDetailsCount != itemsId.Count)
                {
                    throw new DataValidationFailedException("Some products were not found");
                }
                return null;

            default:
                throw new DataValidationFailedException("Invalid product type");
        }
    }
}