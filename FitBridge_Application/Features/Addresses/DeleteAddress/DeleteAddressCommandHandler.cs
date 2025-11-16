using System;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Features.Addresses.DeleteAddress;

public class DeleteAddressCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteAddressCommand, bool>
{
    public async Task<bool> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await unitOfWork.Repository<Address>().GetByIdAsync(request.Id);
        if (address == null)
        {
            throw new NotFoundException("Address not found");
        }
        unitOfWork.Repository<Address>().Delete(address);
        await unitOfWork.CommitAsync();
        return true;
    }
}
