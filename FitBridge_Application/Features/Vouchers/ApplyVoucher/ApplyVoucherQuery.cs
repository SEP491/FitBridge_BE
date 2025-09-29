using FitBridge_Application.Dtos.OrderItems;
using FitBridge_Application.Dtos.Vouchers;
using MediatR;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.Vouchers.ApplyVoucher
{
    public class ApplyVoucherQuery : IRequest<ApplyVoucherDto>
    {
        [JsonIgnore]
        public Guid VoucherId { get; set; }

        public List<OrderItemDto> OrderItemDtos { get; set; }
    }
}