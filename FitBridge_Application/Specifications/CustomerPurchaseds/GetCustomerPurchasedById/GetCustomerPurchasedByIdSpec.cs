using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedById;

public class GetCustomerPurchasedByIdSpec : BaseSpecification<CustomerPurchased>
{
    public GetCustomerPurchasedByIdSpec(Guid id) : base(x => x.Id == id
    && x.IsEnabled)
    {
        AddInclude(x => x.OrderItems);
    }
}
