namespace FitBridge_Application.Specifications.Dashboards.GetPendingBalanceDetail
{
    public class GetPendingBalanceDetailParams : BaseParams
    {
        public DateTime? From { get; set; }
        
        public DateTime? To { get; set; }
    }
}
