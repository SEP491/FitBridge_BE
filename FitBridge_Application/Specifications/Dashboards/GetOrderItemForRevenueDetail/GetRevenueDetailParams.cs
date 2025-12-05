namespace FitBridge_Application.Specifications.Dashboards.GetOrderItemForRevenueDetail
{
    public class GetRevenueDetailParams : BaseParams
    {
        public DateTime? From { get; set; }
        
        public DateTime? To { get; set; }
    }
}
