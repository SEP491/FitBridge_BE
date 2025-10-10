using AutoMapper;
using FitBridge_Application.Dtos.Coupons;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Coupons.GetCouponById;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Coupons.GetCouponById
{
    internal class GetCouponByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<GetCouponByIdQuery, GetCouponsDto>
    {
        public async Task<GetCouponsDto> Handle(GetCouponByIdQuery request, CancellationToken cancellationToken)
        {
            var spec = new GetCouponByIdSpecification(request.Id, getActiveOnly: true);
            var couponDto = await unitOfWork.Repository<Coupon>()
                .GetBySpecificationProjectedAsync<GetCouponsDto>(spec, mapper.ConfigurationProvider)
                ?? throw new NotFoundException(nameof(Coupon));

            return couponDto;
        }
    }
}