using System;

namespace FitBridge_Application.Specifications.Addresses.GetAllShopAddress;

public class GetAllShopAddressParams : BaseParams
{
    public string? GoogleAddressSearchTerm { get; set; }
}
