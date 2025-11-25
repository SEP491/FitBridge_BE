using System;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Contracts;
using FitBridge_Domain.Enums.Contracts;
using FitBridge_Domain.Exceptions;
using MediatR;
using FitBridge_Application.Interfaces.Services;

namespace FitBridge_Application.Features.Contracts.ConfirmContract;

public class ConfirmContractCommandHandler(IUnitOfWork _unitOfWork, IApplicationUserService _applicationUserService) : IRequestHandler<ConfirmContractCommand, Guid>
{
    public async Task<Guid> Handle(ConfirmContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await _unitOfWork.Repository<ContractRecord>().GetByIdAsync(request.Id);
        if (contract == null)
        {
            throw new NotFoundException("Contract not found");
        }
        if (contract.ContractStatus != ContractStatus.BothSigned)
        {
            throw new BusinessException("Contract is not signed by both parties");
        }
        var customer = await _applicationUserService.GetByIdAsync(contract.CustomerId, isTracking: true);
        if (customer == null)
        {
            throw new NotFoundException("User not found");
        }
        customer.IsContractSigned = true;
        contract.ContractStatus = ContractStatus.Finished;
        contract.UpdatedAt = DateTime.UtcNow;
        
        _unitOfWork.Repository<ContractRecord>().Update(contract);
        await _unitOfWork.CommitAsync();
        return contract.Id;
    }
}
