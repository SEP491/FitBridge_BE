using System;
using FitBridge_Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.Contracts;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Enums.Contracts;

namespace FitBridge_Application.Features.Contracts.UpdateContract;

public class UpdateContractCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IUploadService _uploadService) : IRequestHandler<UpdateContractCommand, Guid>
{
    public async Task<Guid> Handle(UpdateContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await _unitOfWork.Repository<ContractRecord>().GetByIdAsync(request.ContractId);
        if (contract == null)
        {
            throw new NotFoundException("Contract not found");
        }
        if (request.ContractUrl != null)
        {
            contract.ContractUrl = await _uploadService.UploadFileAsync(request.ContractUrl);
        }
        if (request.CompanySignatureUrl != null)
        {
            contract.CompanySignatureUrl = await _uploadService.UploadFileAsync(request.CompanySignatureUrl);
            contract.ContractStatus = ContractStatus.CompanySigned;
        }
        if (request.CustomerSignatureUrl != null)
        {
            contract.CustomerSignatureUrl = await _uploadService.UploadFileAsync(request.CustomerSignatureUrl);
            contract.ContractStatus = ContractStatus.CustomerSigned;
        }
        if(contract.CompanySignatureUrl != null && contract.CustomerSignatureUrl != null)
        {
            contract.ContractStatus = ContractStatus.BothSigned;
        }
        contract.UpdatedAt = DateTime.UtcNow;
         _unitOfWork.Repository<ContractRecord>().Update(contract);
        await _unitOfWork.CommitAsync();
        return contract.Id;
    }
}
