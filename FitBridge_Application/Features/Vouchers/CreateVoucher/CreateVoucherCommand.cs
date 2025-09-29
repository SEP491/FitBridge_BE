using FitBridge_Application.Dtos.Vouchers;
using MediatR;

namespace FitBridge_Application.Features.Vouchers.CreateFreelancePTVoucher
{
    public class CreateVoucherCommand : IRequest<CreateNewVoucherDto>
    {
        public decimal MaxDiscount { get; set; }

        public double DiscountPercent { get; set; }

        public int Quantity { get; set; }
    }
}