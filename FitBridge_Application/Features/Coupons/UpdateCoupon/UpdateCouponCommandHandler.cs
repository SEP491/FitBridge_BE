using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Services;
using FitBridge_Application.Specifications.Coupons.GetCouponById;
using FitBridge_Application.Specifications.Coupons.GetOverlapCoupons;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Coupons.UpdateCoupon
{
    internal class UpdateCouponCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<UpdateCouponCommand>
    {
        public async Task Handle(UpdateCouponCommand request, CancellationToken cancellationToken)
        {
            var spec = new GetCouponByIdSpecification(request.CouponId);
            var existingCoupon = await unitOfWork.Repository<Coupon>().GetBySpecificationAsync(spec, false)
                ?? throw new NotFoundException(nameof(Coupon));
            if (request.MaxDiscount.HasValue && request.MaxDiscount > 0m)
            {
                existingCoupon.MaxDiscount = request.MaxDiscount.Value;
            }

            if (request.DiscountPercent.HasValue && request.DiscountPercent > 0d)
            {
                existingCoupon.DiscountPercent = request.DiscountPercent.Value;
            }

            if (request.Quantity.HasValue && request.Quantity != 0)
            {
                existingCoupon.Quantity = request.Quantity.Value;
            }

            if (request.IsActive.HasValue)
            {
                existingCoupon.IsActive = request.IsActive.Value;
            }

            unitOfWork.Repository<Coupon>().Update(existingCoupon);
            await unitOfWork.CommitAsync();
        }
    }
}