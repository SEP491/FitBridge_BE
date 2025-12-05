namespace FitBridge_Application.Dtos.Dashboards
{
    public class AvailableBalanceTransactionDto
    {
        public Guid TransactionId { get; set; }

        public decimal TotalProfit { get; set; }

        public DateOnly? ActualDistributionDate { get; set; }

        public string TransactionType { get; set; }

        public Guid? WithdrawalRequestId { get; set; }

        public Guid? OrderItemId { get; set; }

        public string? CourseName { get; set; } // aka. order item name
    }
}