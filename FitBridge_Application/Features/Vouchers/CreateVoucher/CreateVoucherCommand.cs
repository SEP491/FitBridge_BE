using FitBridge_Application.Dtos.Coupons;
using MediatR;

namespace FitBridge_Application.Features.Vouchers.CreateFreelancePTVoucher
{
    public class CreateVoucherCommand : IRequest<CreateNewCouponDto>
    {
        public decimal MaxDiscount { get; set; }

        public double DiscountPercent { get; set; }

        public int Quantity { get; set; }
    }
}