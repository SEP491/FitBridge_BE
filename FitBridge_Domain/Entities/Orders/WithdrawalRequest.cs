using System;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Domain.Entities.Orders;

public class WithdrawalRequest : BaseEntity
{
    public WithdrawalRequestStatus Status { get; set; }

    public decimal Amount { get; set; }

    public string? Note { get; set; }

    public string BankName { get; set; }

    public string AccountName { get; set; }

    public string AccountNumber { get; set; }

    public string? ImageUrl { get; set; }

    public string? Reason { get; set; }

    public Guid AccountId { get; set; }
    public bool IsUserApproved { get; set; }

    public ApplicationUser Account { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}