using System;

namespace FitBridge_Application.Specifications.Accounts.GetExpiredContractUser;

public class GetExpiredContractUserParams : BaseParams
{
    public Guid? CustomerId { get; set; }
}
