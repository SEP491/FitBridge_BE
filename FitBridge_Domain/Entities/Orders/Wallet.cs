using System;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Domain.Entities.Orders;

public class Wallet : BaseEntity
{
    public decimal PendingBalance { get; set; }
    public decimal AvailableBalance { get; set; }
    public ApplicationUser User { get; set; }
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
