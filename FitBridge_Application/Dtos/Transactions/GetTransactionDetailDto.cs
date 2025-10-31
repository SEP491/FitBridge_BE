using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Dtos.Transactions
{
    public class GetTransactionDetailDto
    {
        public Guid Id { get; set; }

        public decimal Amount { get; set; }

        public TransactionStatus Status { get; set; }

        public string? Description { get; set; }

        public decimal? ProfitAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        public string PaymentMethod { get; set; }

        public WithdrawalRequestDto? WithdrawalRequest { get; set; }
    }
}