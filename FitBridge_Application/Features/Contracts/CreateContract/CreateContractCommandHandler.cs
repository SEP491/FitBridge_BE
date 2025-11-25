using System;
using FitBridge_Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using FitBridge_Domain.Entities.Contracts;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Commons.Constants;
using FitBridge_Domain.Enums.Contracts;
using FitBridge_Application.Services;

namespace FitBridge_Application.Features.Contracts.CreateContract;

public class CreateContractCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IApplicationUserService _applicationUserService, SystemConfigurationService _systemConfigurationService) : IRequestHandler<CreateContractCommand, Guid>
{
    public async Task<Guid> Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        var customer = await _applicationUserService.GetByIdAsync(request.CustomerId);
        if (customer == null)
        {
            throw new NotFoundException("Customer not found");
        }
        var customerRole = await _applicationUserService.GetUserRoleAsync(customer);
        if (customerRole != ProjectConstant.UserRoles.GymOwner && customerRole != ProjectConstant.UserRoles.FreelancePT)
        {
            throw new BusinessException("Customer is not a gym owner or freelance pt");
        }
        var contractRecord = _mapper.Map<CreateContractCommand, ContractRecord>(request);
        if (customerRole == ProjectConstant.UserRoles.GymOwner)
        {
            contractRecord.ContractType = ContractType.GymOwner;
        }
        else
        {
            contractRecord.ContractType = ContractType.FreelancePT;
        }
        var commissionRate =(decimal) await _systemConfigurationService.GetSystemConfigurationAutoConvertDataTypeAsync(ProjectConstant.SystemConfigurationKeys.CommissionRate);
        contractRecord.CommissionPercentage = (int)(commissionRate * 100);
        contractRecord.ContractStatus = ContractStatus.Created;
        _unitOfWork.Repository<ContractRecord>().Insert(contractRecord);
        await _unitOfWork.CommitAsync();
        return contractRecord.Id;
    }
}
