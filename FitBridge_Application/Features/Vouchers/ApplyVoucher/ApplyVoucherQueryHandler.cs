using FitBridge_Application.Dtos.Vouchers;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Orders.GetOrderByVoucherId;
using FitBridge_Application.Specifications.Transactions;
using FitBridge_Application.Specifications.Vouchers.GetVoucherById;
using FitBridge_Application.Specifications.Vouchers.GetVoucherToApply;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Vouchers.ApplyVoucher
{
    internal class ApplyVoucherQueryHandler(
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IUserUtil userUtil) : IRequestHandler<ApplyVoucherQuery, ApplyVoucherDto>
    {
        async Task<ApplyVoucherDto> IRequestHandler<ApplyVoucherQuery, ApplyVoucherDto>.Handle(ApplyVoucherQuery request, CancellationToken cancellationToken)
        {
            var accountId = userUtil.GetAccountId(httpContextAccessor.HttpContext) ?? throw new NotFoundException(nameof(ApplicationUser));
            var voucherSpec = new GetVoucherToApplySpecification(request.VoucherId);
            var voucher = await unitOfWork.Repository<Voucher>().GetBySpecificationAsync(voucherSpec)
                ?? throw new NotFoundException(nameof(Voucher));
            var orderSpec = new GetOrderByVoucherAndUserIdSpecification(voucher.Id, accountId);
            var order = await unitOfWork.Repository<Order>().GetBySpecificationAsync(orderSpec);


            if (voucher.Type.Equals(VoucherType.FreelancePT) && order != null)
            {
                throw new InvalidDataException("PT coupon can only be used once");
            }

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