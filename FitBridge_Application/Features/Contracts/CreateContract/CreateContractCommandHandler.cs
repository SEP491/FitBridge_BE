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
        await AggregateContractRecord(contractRecord, customer);
        var commissionRate = (decimal)await _systemConfigurationService.GetSystemConfigurationAutoConvertDataTypeAsync(ProjectConstant.SystemConfigurationKeys.CommissionRate);
        contractRecord.CommissionPercentage = (int)(commissionRate * 100);
        contractRecord.ContractStatus = ContractStatus.Created;
        _unitOfWork.Repository<ContractRecord>().Insert(contractRecord);
        await _unitOfWork.CommitAsync();
        return contractRecord.Id;
    }
    
    public async Task AggregateContractRecord(ContractRecord contractRecord, ApplicationUser customer)
    {
        contractRecord.FullName = customer.FullName;

        contractRecord.IdentityCardNumber = customer.CitizenIdNumber ?? throw new ContractMissingInfoException("Customer identity card number is required please update profile of customer to complete this contract");

        contractRecord.IdentityCardDate = customer.IdentityCardDate ?? throw new ContractMissingInfoException("Customer identity card date is required please update profile of customer to complete this contract");

        contractRecord.IdentityCardPlace = customer.IdentityCardPlace ?? throw new ContractMissingInfoException("Customer identity card place is required please update profile of customer to complete this contract");

        contractRecord.PermanentAddress = customer.CitizenCardPermanentAddress ?? throw new ContractMissingInfoException("Customer permanent address is required please update profile of customer to complete this contract");

        contractRecord.PhoneNumber = customer.PhoneNumber ?? throw new ContractMissingInfoException("Customer phone number is required please update profile of customer to complete this contract");

        contractRecord.TaxCode = customer.TaxCode ?? throw new ContractMissingInfoException("Customer tax code is required please update profile of customer to complete this contract");
    }
}
