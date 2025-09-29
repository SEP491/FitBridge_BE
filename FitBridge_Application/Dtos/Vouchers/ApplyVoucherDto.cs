namespace FitBridge_Application.Dtos.Vouchers
{
    public class ApplyVoucherDto
    {
        public Guid VoucherId { get; set; }

        public decimal VoucherDiscount { get; set; }

        public double DiscountPercent { get; set; }
    }
}