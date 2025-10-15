using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedForFreelancePt;

public class GetCustomerPurchasedForFreelancePtSpec : BaseSpecification<CustomerPurchased>
{
    public GetCustomerPurchasedForFreelancePtSpec(Guid freelancePtId, GetCustomerPurchasedForFreelancePtParams parameters) : base(x =>
    x.ExpirationDate >= DateOnly.FromDateTime(DateTime.UtcNow)
    && x.OrderItems.Any(x => x.FreelancePTPackage != null && x.FreelancePTPackage.PtId == freelancePtId)
    && x.IsEnabled)
    {
        if (parameters.DoApplyPaging)
        {
            AddPaging((parameters.Page - 1) * parameters.Size, parameters.Size);
        }
        else
        {
            parameters.Size = -1;
            parameters.Page = -1;
        }
    }
}
