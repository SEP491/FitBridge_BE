using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Transactions;
using FitBridge_Application.Specifications.Transactions.GetTransactionByPtId;
using MediatR;

namespace FitBridge_Application.Features.Transactions.GetFreelancePtTransactions
{
    public class GetFreelancePtTransactionsQuery(GetTransactionByPtIdParam parameters) : IRequest<PagingResultDto<GetTransactionsDto>>
    {
        public GetTransactionByPtIdParam Parameters { get; set; } = parameters;
    }
}