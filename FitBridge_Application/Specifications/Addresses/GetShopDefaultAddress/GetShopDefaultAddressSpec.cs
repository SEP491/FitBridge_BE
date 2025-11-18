using System;
using FitBridge_Domain.Entities.Accounts;

namespace FitBridge_Application.Specifications.Addresses.GetShopDefaultAddress;

public class GetShopDefaultAddressSpec : BaseSpecification<Address>
{
    public GetShopDefaultAddressSpec() : base(x => x.IsShopDefaultAddress == true)
    {
    }
}
