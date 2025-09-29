using FitBridge_Application.Dtos.Vouchers;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Vouchers.GetVoucherById;
using FitBridge_Application.Specifications.Vouchers.GetVoucherToApply;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Vouchers.ApplyVoucher
{
    internal class ApplyVoucherCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<ApplyVoucherCommand, ApplyVoucherDto>
    {
        async Task<ApplyVoucherDto> IRequestHandler<ApplyVoucherCommand, ApplyVoucherDto>.Handle(ApplyVoucherCommand request, CancellationToken cancellationToken)
        {
            var spec = new GetVoucherToApplySpecification(request.VoucherId);
            var voucher = await unitOfWork.Repository<Voucher>().GetBySpecificationAsync(spec, asNoTracking: false)
                ?? throw new NotFoundException(nameof(Voucher));

            if (voucher.Quantity - 1 <= 0)
            {
                throw new OutOfStocksException(nameof(Voucher));
            }
            var beforeDiscount = CalculateBeforeDiscount();

            var dto = new ApplyVoucherDto
            {
                VoucherId = voucher.Id,
                VoucherDiscount = CalculateTotalPrice(),
                DiscountPercent = voucher.DiscountPercent,
            };

            voucher.Quantity--;

            unitOfWork.Repository<Voucher>().Update(voucher);
            await unitOfWork.CommitAsync();

            return dto;

            decimal CalculateBeforeDiscount()
            {
                return request.OrderItemDtos.Sum(item => item.Price * item.Quantity);
            }
            decimal CalculateTotalPrice()
            {
                var discountAmount = Math.Max((beforeDiscount * (decimal)voucher.DiscountPercent) / 100, voucher.MaxDiscount);
                return beforeDiscount - discountAmount;
            }
        }
    }
}