using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.BodyMeasurements;
using FitBridge_Application.Specifications.BodyMeasurements.GetBodyMeasurementRecordsByCustomerPurchasedId;
using MediatR;

namespace FitBridge_Application.Features.BodyMeasurements.GetBodyMeasurementRecordsByCustomerPurchasedId;

public class GetBodyMeasurementRecordsByCustomerPurchasedIdQuery : IRequest<PagingResultDto<BodyMeasurementRecordDto>>
{
    public GetBodyMeasurementRecordsByCustomerPurchasedIdParams Params { get; set; }
    public Guid CustomerPurchasedId { get; set; }

}
