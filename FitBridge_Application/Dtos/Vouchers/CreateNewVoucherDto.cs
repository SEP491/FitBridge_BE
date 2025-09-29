using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Dtos.Vouchers
{
    public class CreateNewVoucherDto
    {
        public Guid Id { get; set; }

        public decimal MaxDiscount { get; set; }

        public VoucherType Type { get; set; }

        public double DiscountPercent { get; set; }

        public int Quantity { get; set; }

        public Guid CreatorId { get; set; }
    }
}