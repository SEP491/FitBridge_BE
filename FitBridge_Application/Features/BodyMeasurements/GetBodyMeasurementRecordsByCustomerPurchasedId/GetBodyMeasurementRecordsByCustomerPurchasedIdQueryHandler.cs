using System;
using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.BodyMeasurements;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.BodyMeasurements.GetBodyMeasurementRecordsByCustomerPurchasedId;
using FitBridge_Domain.Entities.Trainings;
using MediatR;

namespace FitBridge_Application.Features.BodyMeasurements.GetBodyMeasurementRecordsByCustomerPurchasedId;

public class GetBodyMeasurementRecordsByCustomerPurchasedIdQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetBodyMeasurementRecordsByCustomerPurchasedIdQuery, PagingResultDto<BodyMeasurementRecordDto>>
{
    public async Task<PagingResultDto<BodyMeasurementRecordDto>> Handle(GetBodyMeasurementRecordsByCustomerPurchasedIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetBodyMeasurementRecordsByCustomerPurchasedSpec(request.Params, request.CustomerPurchasedId);
        var records = await _unitOfWork.Repository<BodyMeasurementRecord>().GetAllWithSpecificationAsync(spec);
        var results = _mapper.Map<IReadOnlyList<BodyMeasurementRecordDto>>(records);
        var totalItems = await _unitOfWork.Repository<BodyMeasurementRecord>().CountAsync(spec);
        return new PagingResultDto<BodyMeasurementRecordDto>(totalItems, results);
    }

}
