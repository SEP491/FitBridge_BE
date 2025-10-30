using System;
using AutoMapper;
using FitBridge_Application.Dtos.BodyMeasurements;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.Accounts;

namespace FitBridge_Application.Features.BodyMeasurements.CreateBodyMeasurement;

public class CreateBodyMeasurementCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<CreateBodyMeasurementCommand, string>
{
    public async Task<string> Handle(CreateBodyMeasurementCommand request, CancellationToken cancellationToken)
    {
        var customerPurchased = await _unitOfWork.Repository<CustomerPurchased>().GetByIdAsync(request.CustomerPurchasedId);
        if (customerPurchased == null)
        {
            throw new NotFoundException("Customer purchased not found");
        }
        if(customerPurchased.ExpirationDate < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new BusinessException("Customer purchased expired");
        }
        var bodyMeasurementRecord = _mapper.Map<CreateBodyMeasurementCommand, BodyMeasurementRecord>(request);
        var userDetail = await _unitOfWork.Repository<UserDetail>().GetByIdAsync(customerPurchased.CustomerId);
        if (userDetail == null)
        {
            throw new NotFoundException("User detail not found");
        }
        userDetail.Biceps = request.Biceps ?? userDetail.Biceps;
        userDetail.ForeArm = request.ForeArm ?? userDetail.ForeArm;
        userDetail.Thigh = request.Thigh ?? userDetail.Thigh;
        userDetail.Calf = request.Calf ?? userDetail.Calf;
        userDetail.Chest = request.Chest ?? userDetail.Chest;
        userDetail.Waist = request.Waist ?? userDetail.Waist;
        userDetail.Hip = request.Hip ?? userDetail.Hip;
        userDetail.Shoulder = request.Shoulder ?? userDetail.Shoulder;
        userDetail.Height = request.Height ?? userDetail.Height;
        userDetail.Weight = request.Weight ?? userDetail.Weight;
        userDetail.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<UserDetail>().Update(userDetail);
        _unitOfWork.Repository<BodyMeasurementRecord>().Insert(bodyMeasurementRecord);
        await _unitOfWork.CommitAsync();
        return bodyMeasurementRecord.Id.ToString();
    }

}
