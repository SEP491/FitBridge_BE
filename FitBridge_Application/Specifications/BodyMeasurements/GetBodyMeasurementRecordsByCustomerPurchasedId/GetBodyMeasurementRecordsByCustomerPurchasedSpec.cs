using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Trainings;

namespace FitBridge_Application.Specifications.BodyMeasurements.GetBodyMeasurementRecordsByCustomerPurchasedId;

public class GetBodyMeasurementRecordsByCustomerPurchasedSpec : BaseSpecification<BodyMeasurementRecord>
{
    public GetBodyMeasurementRecordsByCustomerPurchasedSpec(GetBodyMeasurementRecordsByCustomerPurchasedIdParams parameters, Guid customerPurchasedId) : base(x => x.CustomerPurchasedId == customerPurchasedId)
    {
        AddOrderByDesc(x => x.CreatedAt);
        if (parameters.DoApplyPaging)
        {
            AddPaging(parameters.Size * (parameters.Page - 1), parameters.Size);
        }
        else
        {
            parameters.Size = -1;
            parameters.Page = -1;
        }
    }
}
