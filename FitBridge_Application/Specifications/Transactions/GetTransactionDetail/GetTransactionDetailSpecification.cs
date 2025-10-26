using FitBridge_Application.Dtos.Transactions;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Specifications.Transactions.GetTransactionDetail
{
    public class GetTransactionDetailSpecification : BaseSpecification<Transaction>
    {
        public GetTransactionDetailSpecification(Guid transactionId) : base(x => x.IsEnabled &&
            x.Id == transactionId)
        {
            AddInclude(x => x.PaymentMethod);
            AddInclude(x => x.WithdrawalRequest);
        }
    }
}