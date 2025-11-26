using System;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Contracts;
using FitBridge_Domain.Enums.Contracts;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Contracts.DeleteContract;

public class DeleteContractCommandHandler(IUnitOfWork _unitOfWork) : IRequestHandler<DeleteContractCommand, bool>
{
    public async Task<bool> Handle(DeleteContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await _unitOfWork.Repository<ContractRecord>().GetByIdAsync(request.Id);
        if (contract == null)
        {
            throw new NotFoundException("Contract not found");
        }
        if(contract.ContractStatus == ContractStatus.Finished)
        {
            throw new BusinessException("Contract is already finished and cannot be deleted");
        }
        _unitOfWork.Repository<ContractRecord>().SoftDelete(contract);
        await _unitOfWork.CommitAsync();
        return true;
    }
}
