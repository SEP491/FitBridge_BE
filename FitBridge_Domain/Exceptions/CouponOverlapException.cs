namespace FitBridge_Domain.Exceptions
{
    public class CouponOverlapException(string overlappedCouponCode) : BusinessException($"Expiration date overlapped with coupon code: {overlappedCouponCode}")
    {
    }
}