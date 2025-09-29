using MediatR;

namespace FitBridge_Application.Features.Vouchers.RemoveVoucher
{
    public class RemoveVoucherCommand : IRequest
    {
        public Guid VoucherId { get; set; }
    }
}