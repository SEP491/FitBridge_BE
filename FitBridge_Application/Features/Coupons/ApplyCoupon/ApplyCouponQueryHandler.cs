using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos.Coupons;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Coupons.GetCouponToApply;
using FitBridge_Application.Specifications.FreelancePtPackages.GetFreelancePTPackagesByIds;
using FitBridge_Application.Specifications.Orders.GetOrderByCouponAndUserId;
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
        IApplicationUserService applicationUserService,
        IHttpContextAccessor httpContextAccessor,
        IUserUtil userUtil) : IRequestHandler<ApplyCouponQuery, ApplyCouponDto>
    {
        async Task<ApplyCouponDto> IRequestHandler<ApplyCouponQuery, ApplyCouponDto>.Handle(ApplyCouponQuery request, CancellationToken cancellationToken)
        {
            var accountId = userUtil.GetAccountId(httpContextAccessor.HttpContext) ?? throw new NotFoundException(nameof(ApplicationUser));
            var couponSpec = new GetCouponToApplySpecification(request.CouponCode);
            var coupon = await unitOfWork.Repository<Coupon>().GetBySpecificationAsync(couponSpec)
                ?? throw new NotFoundException(nameof(Coupon));

            var creatorRole = await applicationUserService.GetUserRoleAsync(coupon.Creator);
            if (request.IsFreelancePtCoupon && creatorRole.Equals(ProjectConstant.UserRoles.Admin)
                || !request.IsFreelancePtCoupon && creatorRole.Equals(ProjectConstant.UserRoles.FreelancePT))
            {
                throw new CouponNotApplicableException(coupon.CouponCode);
            }

            if (coupon.Quantity - 1 <= 0)
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

            await ValidateFreelancePtCoupon(request.ItemsId, coupon.CreatorId);

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

        private async Task ValidateFreelancePtCoupon(List<Guid> itemsId, Guid couponCreatorId)
        {
            if (itemsId.Count > 0)
            {
                throw new InvalidDataException("No items provided for freelance PT coupon validation");
            }

            var packages = await unitOfWork.Repository<FreelancePTPackage>()
                .GetAllWithSpecificationAsync(new GetFreelancePTPackagesByIdsSpec(itemsId));

            var invalidPackages = packages.Where(p => p.PtId != couponCreatorId).ToList();

            if (invalidPackages.Count > 0)
            {
                throw new InvalidDataException("This coupon can only be applied to packages from the personal trainer who created it");
            }
        }
    }
}