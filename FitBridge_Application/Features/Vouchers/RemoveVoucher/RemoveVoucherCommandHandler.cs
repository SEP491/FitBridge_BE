using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Vouchers;
using FitBridge_Application.Specifications.Vouchers.GetVoucherById;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Vouchers.RemoveVoucher
{
    internal class RemoveVoucherCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<RemoveVoucherCommand>
    {
        public async Task Handle(RemoveVoucherCommand request, CancellationToken cancellationToken)
        {
            var spec = new GetVoucherByIdSpecification(request.VoucherId);
            var existingVoucher = await unitOfWork.Repository<Voucher>().GetBySpecificationAsync(spec, false)
                ?? throw new NotFoundException(nameof(Voucher));

            existingVoucher.IsActive = false;
            unitOfWork.Repository<Voucher>().SoftDelete(existingVoucher);

            await unitOfWork.CommitAsync();
        }
    }
}