namespace FitBridge_Domain.Exceptions
{
    public class CouponNotApplicableException(string couponCode) : BusinessException($"Coupon {couponCode} is not applicable with the product")
    {
    }
}