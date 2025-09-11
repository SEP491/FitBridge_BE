using System;
using FitBridge_Domain.Entities;

namespace FitBridge_Domain.Entities.Orders;

public class PaymentMethod : BaseEntity
{
    public MethodType MethodType { get; set; }
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

public enum MethodType
{
    PayOs
}
