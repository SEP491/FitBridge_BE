using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Vouchers;
using FitBridge_Application.Specifications.Vouchers.GetVoucherById;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Vouchers.UpdateVoucher
{
    internal class UpdateVoucherCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<UpdateVoucherCommand>
    {
        public async Task Handle(UpdateVoucherCommand request, CancellationToken cancellationToken)
        {
            var spec = new GetVoucherByIdSpecification(request.VoucherId);
            var existingVoucher = await unitOfWork.Repository<Coupon>().GetBySpecificationAsync(spec, false)
                ?? throw new NotFoundException(nameof(Coupon));
            if (request.MaxDiscount.HasValue && request.MaxDiscount > 0m)
            {
                existingVoucher.MaxDiscount = request.MaxDiscount.Value;
            }

            if (request.DiscountPercent.HasValue && request.DiscountPercent > 0d)
            {
                existingVoucher.DiscountPercent = request.DiscountPercent.Value;
            }

            if (request.Quantity.HasValue && request.Quantity != 0)
            {
                existingVoucher.Quantity += request.Quantity.Value;
            }

            if (request.IsActive.HasValue)
            {
                existingVoucher.IsActive = request.IsActive.Value;
            }

            unitOfWork.Repository<Coupon>().Update(existingVoucher);
            await unitOfWork.CommitAsync();
        }
    }
}