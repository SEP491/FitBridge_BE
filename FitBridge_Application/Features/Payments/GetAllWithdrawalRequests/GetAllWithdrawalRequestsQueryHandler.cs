using AutoMapper;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Payments;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Payments.GetAllWithdrawalRequests;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Payments.GetAllWithdrawalRequests
{
    internal class GetAllWithdrawalRequestsQueryHandler(
        IUserUtil userUtil,
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<GetAllWithdrawalRequestsQuery, PagingResultDto<GetWithdrawalRequestResponseDto>>
    {
        public async Task<PagingResultDto<GetWithdrawalRequestResponseDto>> Handle(GetAllWithdrawalRequestsQuery request, CancellationToken cancellationToken)
        {
            var accountId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                ?? throw new NotFoundException(nameof(ApplicationUser));

            var userRole = userUtil.GetUserRole(httpContextAccessor.HttpContext);

            if (userRole != ProjectConstant.UserRoles.Admin)
            {
                request.Params.AccountId = accountId;
            }

            var spec = new GetAllWithdrawalRequestsSpec(request.Params);

            var withdrawalRequests = await unitOfWork.Repository<WithdrawalRequest>()
                .GetAllWithSpecificationProjectedAsync<GetWithdrawalRequestResponseDto>(spec, mapper.ConfigurationProvider);

            var totalCount = await unitOfWork.Repository<WithdrawalRequest>()
                .CountAsync(spec);

            return new PagingResultDto<GetWithdrawalRequestResponseDto>(totalCount, withdrawalRequests);
        }
    }
}