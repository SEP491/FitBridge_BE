using FitBridge_Application.Dtos.OrderItems;
using FitBridge_Application.Dtos.Vouchers;
using MediatR;

namespace FitBridge_Application.Features.Vouchers.ApplyVoucher
{
    public class ApplyVoucherCommand : IRequest<ApplyVoucherDto>
    {
        public Guid VoucherId { get; set; }

        public List<OrderItemDto> OrderItemDtos { get; set; }
    }
}