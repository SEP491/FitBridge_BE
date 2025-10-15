namespace FitBridge_Application.Dtos.Templates
{
    public class NewCouponModel(
        string titleCouponCode,
        string bodyGifterName,
        string bodyCouponCode) : IBaseTemplateModel
    {
        public string TitleCouponCode { get; set; } = titleCouponCode;

        public string BodyGifterName { get; set; } = bodyGifterName;

        public string BodyCouponCode { get; set; } = bodyCouponCode;
    }
}