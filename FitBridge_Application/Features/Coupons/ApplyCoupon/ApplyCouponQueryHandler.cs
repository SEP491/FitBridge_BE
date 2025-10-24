using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos.Coupons;
using FitBridge_Application.Features.Payments.ApproveWithdrawalRequest;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Coupons.GetCouponToApply;
using FitBridge_Application.Specifications.FreelancePtPackages.GetFreelancePTPackagesByIds;
using FitBridge_Application.Specifications.GymCourses.GetGymCourseByIds;
using FitBridge_Application.Specifications.Orders.GetOrderByCouponAndUserId;
using FitBridge_Application.Specifications.ProductDetails.GetProductDetailsByIds;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;
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

            if (coupon.Quantity - coupon.NumberOfUsedCoupon <= 0)
            {
                throw new OutOfStocksException(nameof(Coupon));
            }

            if (coupon.ExpirationDate < DateOnly.FromDateTime(DateTime.UtcNow) || !coupon.IsActive)
            {
                throw new ExpiredException(coupon.CouponCode);
            }

            var orderSpec = new GetOrderByCouponAndUserIdSpecification(coupon.Id, accountId);
            var order = await unitOfWork.Repository<Order>().GetBySpecificationAsync(orderSpec);

            if (order != null)
            {
                throw new CouponAlreadyAppliedException(coupon.CouponCode);
            }

            var productCreatorId = await ValidateProductSimilarityAsync(request.ItemsId, request.ProductType);

            if (coupon.CreatorId != productCreatorId)
            {
                throw new DataValidationFailedException("This coupon can only be applied to products of the same creator");
            }

            var dto = new ApplyCouponDto
            {
                Id = coupon.Id,
                DiscountAmount = CalculateTotalPrice(request.TotalPrice, coupon),
                DiscountPercent = coupon.DiscountPercent,
            };

            return dto;
        }

        private static decimal CalculateTotalPrice(decimal totalPrice, Coupon coupon)
        {
            var discountAmount = Math.Max(totalPrice * (decimal)coupon.DiscountPercent / 100, coupon.MaxDiscount);
            return totalPrice - discountAmount;
        }

        private async Task<Guid?> ValidateProductSimilarityAsync(List<Guid> itemsId, string productType)
        {
            // all products should be of the same type, so if the fetched products count mismatched
            // with itemsId count, then at least one product does not match the rest
            switch (productType)
            {
                case nameof(GymCourse):
                    {
                        var gymCoursesCount = await unitOfWork.Repository<GymCourse>()
                            .CountAsync(new GetGymCourseByIdsSpec(itemsId));

                        if (gymCoursesCount > 0 && gymCoursesCount != itemsId.Count)
                        {
                            throw new DataValidationFailedException("This coupon can only be applied to products of the same type or creator does");
                        }
                        var creatorId = (await unitOfWork.Repository<GymCourse>()
                            .GetByIdAsync(itemsId.First())).GymOwnerId;
                        return creatorId;
                    }

                case nameof(FreelancePTPackage):
                    {
                        var packagesCount = await unitOfWork.Repository<FreelancePTPackage>()
                            .CountAsync(new GetFreelancePTPackagesByIdsSpec(itemsId));

                        if (packagesCount > 0 && packagesCount != itemsId.Count)
                        {
                            throw new DataValidationFailedException("This coupon can only be applied to products of the same type");
                        }
                        var creatorId = (await unitOfWork.Repository<FreelancePTPackage>()
                            .GetByIdAsync(itemsId.First())).PtId;
                        return creatorId;
                    }
                case nameof(Product):
                    var productDetailsCount = await unitOfWork.Repository<ProductDetail>()
                        .CountAsync(new GetProductDetailsByIdsSpec(itemsId));

                    if (productDetailsCount > 0 && productDetailsCount != itemsId.Count)
                    {
                        throw new DataValidationFailedException("This coupon can only be applied to products of the same type");
                    }
                    return null;

                default:
                    throw new DataValidationFailedException("Invalid product type");
            }
        }
    }
}