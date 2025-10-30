using System;
using MediatR;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Features.BodyMeasurements.DeleteBodyMeasurement;

public class DeleteBodyMeasurementCommandHandler(IUnitOfWork _unitOfWork) : IRequestHandler<DeleteBodyMeasurementCommand, bool>
{
    public async Task<bool> Handle(DeleteBodyMeasurementCommand request, CancellationToken cancellationToken)
    {
        var bodyMeasurementRecord = await _unitOfWork.Repository<BodyMeasurementRecord>().GetByIdAsync(request.Id);
        if (bodyMeasurementRecord == null)
        {
            throw new NotFoundException("Body measurement record not found");
        }
        _unitOfWork.Repository<BodyMeasurementRecord>().Delete(bodyMeasurementRecord);
        await _unitOfWork.CommitAsync();
        return true;
    }

}
