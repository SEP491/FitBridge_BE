using MediatR;

namespace FitBridge_Application.Features.Vouchers.UpdateVoucher
{
    public class UpdateVoucherCommand : IRequest
    {
        public Guid VoucherId { get; set; }

        public decimal? MaxDiscount { get; set; }

        public double? DiscountPercent { get; set; }

        public int? Quantity { get; set; }

        public bool? IsActive { get; set; }
    }
}