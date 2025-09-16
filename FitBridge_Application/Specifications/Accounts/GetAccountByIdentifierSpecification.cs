using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Specifications.Accounts;

public class GetAccountByIdentifierSpecification : BaseSpecification<ApplicationUser>
{
    public GetAccountByIdentifierSpecification(string identifier) : base(x => x.Email == identifier || x.PhoneNumber == identifier)
    {
    }
}