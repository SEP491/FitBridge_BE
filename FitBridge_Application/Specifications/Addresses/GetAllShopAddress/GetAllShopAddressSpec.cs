using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Application.Specifications.Addresses.GetAllShopAddress;
namespace FitBridge_Application.Specifications.Addresses.GetAllShopAddress;

public class GetAllShopAddressSpec : BaseSpecification<Address>
{
    public GetAllShopAddressSpec(List<Guid> adminAccountIds, GetAllShopAddressParams? parameters = null, Guid? exceptAddressId = null) : base(x =>
    adminAccountIds.Contains(x.CustomerId)
    && (parameters == null || parameters.GoogleAddressSearchTerm == null || x.GoogleMapAddressString.ToLower().Contains(parameters.GoogleAddressSearchTerm.ToLower()))
    && x.IsEnabled
    && (exceptAddressId == null || x.Id != exceptAddressId))
    {
        if(parameters != null)
        {
            if(parameters.DoApplyPaging)
            {
                AddPaging(parameters.Size * (parameters.Page - 1), parameters.Size);
            } else {
                parameters.Size = -1;
                parameters.Page = -1;
            }
        }
    }
}
