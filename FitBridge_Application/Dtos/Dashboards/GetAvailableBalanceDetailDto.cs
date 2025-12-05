namespace FitBridge_Application.Dtos.Dashboards
{
    public class GetAvailableBalanceDetailDto
    {
        public IReadOnlyList<AvailableBalanceTransactionDto> TransactionDtos { get; set; }

        public decimal GrandTotal { get; set; }
    }
}