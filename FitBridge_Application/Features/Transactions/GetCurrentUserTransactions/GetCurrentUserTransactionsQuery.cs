using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Transactions;
using FitBridge_Application.Specifications.Transactions.GetCurrentUserTransactions;
using MediatR;

namespace FitBridge_Application.Features.Transactions.GetCurrentUserTransactions
{
    public class GetCurrentUserTransactionsQuery(GetCurrentUserTransactionsParam parameters) : IRequest<PagingResultDto<GetTransactionsDto>>
    {
        public GetCurrentUserTransactionsParam Parameters { get; set; } = parameters;
    }
}