using FitBridge_Application.Dtos.Transactions;
using MediatR;

namespace FitBridge_Application.Features.Transactions.GetTransactionDetail
{
    public class GetTransactionDetailQuery : IRequest<GetTransactionDetailDto>
    {
        public Guid TransactionId { get; set; }
    }
}