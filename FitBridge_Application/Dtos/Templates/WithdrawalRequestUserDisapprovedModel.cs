namespace FitBridge_Application.Dtos.Templates
{
    public class WithdrawalRequestUserDisapprovedModel : IBaseTemplateModel
    {
        public string TitleRequesterName { get; set; }

        public string BodyRequesterName { get; set; }

        public decimal BodyAmount { get; set; }
    }
}