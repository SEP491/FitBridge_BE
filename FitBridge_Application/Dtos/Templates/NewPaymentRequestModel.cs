namespace FitBridge_Application.Dtos.Templates
{
    public class NewPaymentRequestModel : IBaseTemplateModel
    {
        public string TitleRequesterName { get; set; }

        public string BodyRequesterName { get; set; }

        public decimal BodyAmmount { get; set; }
    }
}