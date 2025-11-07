using System;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Systems;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.SystemConfigurations;

public class UpdateSystemConfigurationCommandHandler(IUnitOfWork _unitOfWork) : IRequestHandler<UpdateSystemConfigurationCommand, bool>
{
    public async Task<bool> Handle(UpdateSystemConfigurationCommand request, CancellationToken cancellationToken)
    {
        var systemConfiguration = await _unitOfWork.Repository<SystemConfiguration>().GetByIdAsync(request.Id);
        if (systemConfiguration == null)
        {
            throw new NotFoundException(nameof(SystemConfiguration));
        }
        systemConfiguration.Value = request.Value ?? systemConfiguration.Value;
        systemConfiguration.Description = request.Description ?? systemConfiguration.Description;
        _unitOfWork.Repository<SystemConfiguration>().Update(systemConfiguration);
        await _unitOfWork.CommitAsync();
        return true;
    }

}
