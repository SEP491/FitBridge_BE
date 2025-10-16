namespace FitBridge_Domain.Exceptions
{
    public class CouponAlreadyAppliedException(string couponCode) : BusinessException($"Coupon {couponCode} has been already applied")
    {
    }
}