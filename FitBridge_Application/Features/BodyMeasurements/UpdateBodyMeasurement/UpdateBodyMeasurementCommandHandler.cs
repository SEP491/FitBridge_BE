using System;
using AutoMapper;
using FitBridge_Application.Dtos.BodyMeasurements;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.BodyMeasurements.GetBodyMeasurementRecordsByCustomerPurchasedId;
using MediatR;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Features.BodyMeasurements.UpdateBodyMeasurement;

public class UpdateBodyMeasurementCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<UpdateBodyMeasurementCommand, BodyMeasurementRecordDto>
{
    public async Task<BodyMeasurementRecordDto> Handle(UpdateBodyMeasurementCommand request, CancellationToken cancellationToken)
    {
        var bodyMeasurementRecord = await _unitOfWork.Repository<BodyMeasurementRecord>().GetByIdAsync(request.Id);
        var allBodyMeasurementRecords = await _unitOfWork.Repository<BodyMeasurementRecord>().GetAllWithSpecificationAsync(new GetBodyMeasurementRecordsByCustomerPurchasedSpec(new GetBodyMeasurementRecordsByCustomerPurchasedIdParams { DoApplyPaging = false }, bodyMeasurementRecord.CustomerPurchasedId));

        var latestBodyMeasurementRecord = allBodyMeasurementRecords.OrderByDescending(x => x.CreatedAt).FirstOrDefault();
        if (bodyMeasurementRecord.CreatedAt == latestBodyMeasurementRecord.CreatedAt)
        {
            await UpdateUserDetailByLatestBodyMeasurementRecord(bodyMeasurementRecord.CustomerPurchasedId, request);
        }
        bodyMeasurementRecord.Biceps = request.Biceps ?? bodyMeasurementRecord.Biceps;
        bodyMeasurementRecord.ForeArm = request.ForeArm ?? bodyMeasurementRecord.ForeArm;
        bodyMeasurementRecord.Thigh = request.Thigh ?? bodyMeasurementRecord.Thigh;
        bodyMeasurementRecord.Calf = request.Calf ?? bodyMeasurementRecord.Calf;
        bodyMeasurementRecord.Chest = request.Chest ?? bodyMeasurementRecord.Chest;
        bodyMeasurementRecord.Waist = request.Waist ?? bodyMeasurementRecord.Waist;
        bodyMeasurementRecord.Hip = request.Hip ?? bodyMeasurementRecord.Hip;
        bodyMeasurementRecord.Shoulder = request.Shoulder ?? bodyMeasurementRecord.Shoulder;
        bodyMeasurementRecord.Height = request.Height ?? bodyMeasurementRecord.Height;
        bodyMeasurementRecord.Weight = request.Weight ?? bodyMeasurementRecord.Weight;
        bodyMeasurementRecord.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<BodyMeasurementRecord>().Update(bodyMeasurementRecord);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<BodyMeasurementRecordDto>(bodyMeasurementRecord);
    }

    public async Task UpdateUserDetailByLatestBodyMeasurementRecord(Guid customerPurchasedId, UpdateBodyMeasurementCommand request)
    {
        var customerPurchased = await _unitOfWork.Repository<CustomerPurchased>().GetByIdAsync(customerPurchasedId);
        if (customerPurchased == null)
        {
            throw new NotFoundException("Customer purchased not found");
        }
        var customerId = customerPurchased.CustomerId;
        var userDetail = await _unitOfWork.Repository<UserDetail>().GetByIdAsync(customerId);
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
    }
}
