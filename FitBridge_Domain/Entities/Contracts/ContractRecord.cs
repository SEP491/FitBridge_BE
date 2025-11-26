using System;
using FitBridge_Domain.Enums.Contracts;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Domain.Entities.Contracts;

public class ContractRecord : BaseEntity
{
    public Guid CustomerId { get; set; }
    public ContractType ContractType { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string FullName { get; set; }
    public string IdentityCardNumber { get; set; }
    public DateOnly IdentityCardDate { get; set; }
    public string IdentityCardPlace { get; set; }
    public string PermanentAddress { get; set; }
    public string PhoneNumber { get; set; }
    public string TaxCode { get; set; }
    public string BusinessAddress { get; set; }
    public int CommissionPercentage { get; set; }
    public string? ContractUrl { get; set; }
    public string? CompanySignatureUrl { get; set; }
    public string? CustomerSignatureUrl { get; set; }
    public string? ContactEmail { get; set; }
    public string? GymName { get; set; }
    public List<string>? ExtraRules { get; set; } = new List<string>();
    public ContractStatus ContractStatus { get; set; }
    public ApplicationUser Customer { get; set; }
}
