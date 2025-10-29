namespace FitBridge_Application.Dtos.Templates
{
    public class RefundedItemModel : IBaseTemplateModel
    {
        public string BodyRequesterName { get; set; }

        public decimal BodyAmount { get; set; }
    }
}