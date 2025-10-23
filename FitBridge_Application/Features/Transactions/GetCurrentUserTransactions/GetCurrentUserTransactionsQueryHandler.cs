using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Transactions;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Transactions.GetCurrentUserTransactions;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Transactions.GetCurrentUserTransactions
{
    internal class GetCurrentUserTransactionsQueryHandler(
        IUserUtil userUtil,
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<GetCurrentUserTransactionsQuery, PagingResultDto<GetTransactionsDto>>
    {
        public async Task<PagingResultDto<GetTransactionsDto>> Handle(GetCurrentUserTransactionsQuery request, CancellationToken cancellationToken)
        {
            var accountId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                    ?? throw new NotFoundException(nameof(ApplicationUser));

            var spec = new GetCurrentUserTransactionsSpec(request.Parameters, accountId);
            var transactions = await unitOfWork.Repository<Transaction>()
                .GetAllWithSpecificationProjectedAsync<GetTransactionsDto>(spec, mapper.ConfigurationProvider);
            var totalCount = await unitOfWork.Repository<Transaction>()
                .CountAsync(spec);

            return new PagingResultDto<GetTransactionsDto>(totalCount, transactions);
        }
    }
}