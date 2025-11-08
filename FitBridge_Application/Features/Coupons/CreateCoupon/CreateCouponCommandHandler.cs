using AutoMapper;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos.Coupons;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Services;
using FitBridge_Application.Specifications.Coupons.GetOverlapCoupons;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Coupons.CreateCoupon
{
    internal class CreateCouponCommandHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IUserUtil userUtil,
        IHttpContextAccessor httpContext,
        IApplicationUserService applicationUserService) : IRequestHandler<CreateCouponCommand, CreateNewCouponDto>
    {
        public async Task<CreateNewCouponDto> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
        {
            var creatorId = userUtil.GetAccountId(httpContext.HttpContext)
                ?? throw new NotFoundException("User not found");

            var creator = await applicationUserService.GetByIdAsync(creatorId) ?? throw new NotFoundException(nameof(ApplicationUser));

            var creatorRole = await applicationUserService.GetUserRoleAsync(creator);
            var couponType = ExtractCouponType(creatorRole);

            var newCoupon = new Coupon
            {
                CouponCode = request.CouponCode,
                MaxDiscount = request.MaxDiscount,
                Type = couponType,
                DiscountPercent = request.DiscountPercent,
                Quantity = request.Quantity,
                CreatorId = creatorId,
                IsActive = false,
                StartDate = request.StartDate,
                ExpirationDate = request.ExpirationDate,
                Id = Guid.NewGuid()
            };

            var overlappedCoupons = await GetOverlappedCoupons(newCoupon.StartDate, creatorId);
            if (overlappedCoupons.Count > 0)
            {
                throw new CouponOverlapException(overlappedCoupons[0].CouponCode);
            }

            unitOfWork.Repository<Coupon>().Insert(newCoupon);

            try
            {
                await unitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                throw new DuplicateException("Coupon code already exists");
            }

            return mapper.Map<CreateNewCouponDto>(newCoupon);
        }

        private static CouponType ExtractCouponType(string creatorRole)
        {
            return creatorRole switch
            {
                ProjectConstant.UserRoles.GymOwner => CouponType.GymOwner,
                ProjectConstant.UserRoles.Admin => CouponType.System,
                ProjectConstant.UserRoles.FreelancePT => CouponType.FreelancePT,
                _ => throw new DataValidationFailedException("Unknown creator role for coupon"),
            };
        }

        private async Task<IReadOnlyList<Coupon>> GetOverlappedCoupons(DateOnly expirationDate, Guid creatorId)
        {
            var spec = new GetOverlapCouponsSpec(expirationDate, creatorId);
            var overlapCoupons = await unitOfWork.Repository<Coupon>().GetAllWithSpecificationAsync(spec);

            return overlapCoupons;
        }
    }
}