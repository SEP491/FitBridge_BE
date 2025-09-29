using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Coupons.GetCouponById;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Coupons.RemoveCoupon
{
    internal class RemoveCouponCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<RemoveCouponCommand>
    {
        public async Task Handle(RemoveCouponCommand request, CancellationToken cancellationToken)
        {
            var spec = new GetCouponByIdSpecification(request.CouponId);
            var existingCoupon = await unitOfWork.Repository<Coupon>().GetBySpecificationAsync(spec, false)
                ?? throw new NotFoundException(nameof(Coupon));

            existingCoupon.IsActive = false;
            unitOfWork.Repository<Coupon>().SoftDelete(existingCoupon);

            await unitOfWork.CommitAsync();
        }
    }
}