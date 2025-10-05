using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Dtos.Transactions
{
    public class GetTransactionsDto
    {
        public Guid Id { get; set; }

        public TransactionStatus Status { get; set; }

        public decimal Amount { get; set; }

        public long OrderCode { get; set; }

        public TransactionType TransactionType { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}