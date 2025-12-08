namespace FitBridge_Application.Dtos.Dashboards
{
    public class TransactionDetailDto
    {
        public Guid TransactionId { get; set; }

        public long OrderCode { get; set; }

        public DateTime TransactionDate { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string? Description { get; set; }
    }
}
