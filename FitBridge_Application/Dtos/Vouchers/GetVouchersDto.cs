namespace FitBridge_Application.Dtos.Vouchers
{
    public class GetVouchersDto
    {
        public Guid Id { get; set; }

        public decimal MaxDiscount { get; set; }

        public double DiscountPercent { get; set; }

        public int Quantity { get; set; }

        public bool IsActive { get; set; }
    }
}