using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Accounts;

namespace FitBridge_Application.Specifications.Addresses.GetAllCustomerAddress;

public class GetAllCustomerAddressesSpec : BaseSpecification<Address>
{
    public GetAllCustomerAddressesSpec(Guid customerId) : base(x => x.CustomerId == customerId)
    {
        AddInclude(x => x.Customer);
    }
}
