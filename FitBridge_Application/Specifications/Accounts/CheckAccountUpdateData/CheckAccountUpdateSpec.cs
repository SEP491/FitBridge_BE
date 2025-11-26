using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Specifications.Accounts.CheckAccountUpdateData;

public class CheckAccountUpdateSpec : BaseSpecification<ApplicationUser>
{
    public CheckAccountUpdateSpec(Guid id, string? taxCode, string? citizenIdNumber) : base(x => x.Id != id
    && (taxCode == null || x.TaxCode == taxCode)
    && (citizenIdNumber == null || x.CitizenIdNumber == citizenIdNumber)
    && x.IsEnabled)
    {
    }
}
