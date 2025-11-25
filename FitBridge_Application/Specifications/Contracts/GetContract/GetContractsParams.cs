using System;

namespace FitBridge_Application.Specifications.Contracts.GetContract;

public class GetContractsParams : BaseParams
{
    public Guid? CustomerId { get; set; }
}
