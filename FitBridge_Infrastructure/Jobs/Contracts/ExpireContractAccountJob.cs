using System;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Entities.Contracts;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Enums.Contracts;
using FitBridge_Domain.Exceptions;
using Quartz;

namespace FitBridge_Infrastructure.Jobs.Contracts;

public class ExpireContractAccountJob(IUnitOfWork _unitOfWork, IApplicationUserService _applicationUserService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var contractId = Guid.Parse(context.JobDetail.JobDataMap.GetString("contractId") ?? throw new NotFoundException("Contract ID not found"));
        var contract = await _unitOfWork.Repository<ContractRecord>().GetByIdAsync(contractId);
        if (contract == null)
        {
            throw new NotFoundException("Contract not found");
        }
        var user = await _applicationUserService.GetByIdAsync(contract.CustomerId, isTracking: true);
        if (user == null)
        {
            throw new NotFoundException($"User not found for contract {contractId}");
        }
        user.IsContractSigned = false;
        await _unitOfWork.CommitAsync();
    }
}
