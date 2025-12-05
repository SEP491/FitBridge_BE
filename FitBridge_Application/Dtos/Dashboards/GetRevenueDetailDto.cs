namespace FitBridge_Application.Dtos.Dashboards
{
    public class GetRevenueDetailDto
    {
        public IReadOnlyList<RevenueOrderItemDto> Items { get; set; } = [];

        public decimal GrandTotal { get; set; }
    }
}