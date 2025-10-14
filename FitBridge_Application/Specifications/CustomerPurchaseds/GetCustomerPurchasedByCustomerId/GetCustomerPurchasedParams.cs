using System;

namespace FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByCustomerId;

public class GetCustomerPurchasedParams : BaseParams
{
    public bool IsOngoingOnly { get; set; } = false;

    public Guid CustomerId { get; set; }
}