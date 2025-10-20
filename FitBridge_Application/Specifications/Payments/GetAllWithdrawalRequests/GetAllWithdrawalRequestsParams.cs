namespace FitBridge_Application.Specifications.Payments.GetAllWithdrawalRequests
{
    public class GetAllWithdrawalRequestsParams : BaseParams
    {
        public Guid? AccountId { get; set; } = null;
    }
}