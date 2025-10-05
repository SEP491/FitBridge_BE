using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Transactions;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Transactions;
using FitBridge_Application.Specifications.Transactions.GetTransactionByPtId;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Transactions.GetFreelancePtTransactions
{
    internal class GetFreelancePtTransactionsQueryHandler(
        IUserUtil userUtil,
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<GetFreelancePtTransactionsQuery, PagingResultDto<GetTransactionsDto>>
    {
        public async Task<PagingResultDto<GetTransactionsDto>> Handle(GetFreelancePtTransactionsQuery request, CancellationToken cancellationToken)
        {
            var accountId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                    ?? throw new NotFoundException(nameof(Transaction));
            var spec = new GetTransactionByPtIdSpec(request.Parameters, accountId);
            var transactions = await unitOfWork.Repository<Transaction>()
                .GetAllWithSpecificationProjectedAsync<GetTransactionsDto>(spec, mapper.ConfigurationProvider);
            var totalCount = await unitOfWork.Repository<Transaction>()
                .CountAsync(spec);

            return new PagingResultDto<GetTransactionsDto>(totalCount, transactions);
        }
    }
}