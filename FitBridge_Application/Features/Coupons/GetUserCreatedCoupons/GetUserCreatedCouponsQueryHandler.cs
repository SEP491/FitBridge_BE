using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Coupons;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Coupons.GetCouponByCreatorId;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Coupons.GetUserCreatedCoupons
{
    internal class GetUserCreatedCouponsQueryHandler(
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork,
        IUserUtil userUtil,
        IMapper mapper) : IRequestHandler<GetUserCreatedCouponsQuery, PagingResultDto<GetCouponsDto>>
    {
        public async Task<PagingResultDto<GetCouponsDto>> Handle(GetUserCreatedCouponsQuery request, CancellationToken cancellationToken)
        {
            var creatorId = userUtil.GetAccountId(httpContextAccessor.HttpContext) ?? throw new NotFoundException(nameof(ApplicationUser));

            var spec = new GetCouponsByCreatorIdSpecification(request.Params, creatorId);
            var couponDtos = await unitOfWork.Repository<Coupon>()
                .GetAllWithSpecificationProjectedAsync<GetCouponsDto>(spec, mapper.ConfigurationProvider);
            var couponTotalCount = await unitOfWork.Repository<Coupon>().CountAsync(spec);

            return new PagingResultDto<GetCouponsDto>(couponTotalCount, couponDtos.ToList());
        }
    }
}