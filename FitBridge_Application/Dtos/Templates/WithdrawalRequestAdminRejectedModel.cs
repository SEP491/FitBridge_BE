namespace FitBridge_Application.Dtos.Templates
{
    public class WithdrawalRequestAdminRejectedModel : IBaseTemplateModel
    {
        public decimal BodyAmount { get; set; }

        public string BodyRejectedBy { get; set; }

        public string BodyReason { get; set; }
    }
}