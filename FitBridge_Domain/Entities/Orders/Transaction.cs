using System;
using System.Transactions;
using FitBridge_Domain.Entities;

namespace FitBridge_Domain.Entities.Orders;

public class Transaction : BaseEntity
{
    public TransactionStatus Status { get; set; }
    public decimal Amount { get; set; }
    public string OrderCode { get; set; }
    public string Description { get; set; }
    public Guid PaymentMethodId { get; set; }
    public Guid? WithdrawalRequestId { get; set; }
    public Guid? OrderId { get; set; }

    public PaymentMethod PaymentMethod { get; set; }
    public WithdrawalRequest? WithdrawalRequest { get; set; }
    public Order? Order { get; set; }
    public TransactionType TransactionType { get; set; }
}

public enum TransactionStatus
{
    Success,
    Failed
}

public enum TransactionType
{
    ProductOrder,
    GymCourse,
    Withdraw,
    ServiceOrder,
}
