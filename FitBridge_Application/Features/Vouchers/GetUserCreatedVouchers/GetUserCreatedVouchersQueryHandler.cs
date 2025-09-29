using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Vouchers;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Vouchers;
using FitBridge_Application.Specifications.Vouchers.GetVoucherByCreatorId;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Vouchers.GetUserVouchers
{
    internal class GetUserCreatedVouchersQueryHandler(
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork,
        IUserUtil userUtil,
        IMapper mapper) : IRequestHandler<GetUserCreatedVouchersQuery, PagingResultDto<GetVouchersDto>>
    {
        public async Task<PagingResultDto<GetVouchersDto>> Handle(GetUserCreatedVouchersQuery request, CancellationToken cancellationToken)
        {
            var creatorId = userUtil.GetAccountId(httpContextAccessor.HttpContext) ?? throw new NotFoundException(nameof(ApplicationUser));

            var spec = new GetVoucherByCreatorIdSpecification(request.Params, creatorId);
            var voucherDtos = await unitOfWork.Repository<Voucher>()
                .GetAllWithSpecificationProjectedAsync<GetVouchersDto>(spec, mapper.ConfigurationProvider);
            var voucherTotalCount = await unitOfWork.Repository<Voucher>().CountAsync(spec);

            return new PagingResultDto<GetVouchersDto>(voucherTotalCount, voucherDtos.ToList());
        }
    }
}