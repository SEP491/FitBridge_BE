namespace FitBridge_Application.Dtos.Dashboards
{
    public class GetPendingBalanceDetailDto
    {
        public IReadOnlyList<PendingBalanceOrderItemDto> PendingBalanceOrders { get; set; } = [];

        public decimal GrandTotal { get; set; }
    }
}