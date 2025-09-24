using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Domain.Entities.Orders;

public class Transaction : BaseEntity
{
    public TransactionStatus Status { get; set; }

    public decimal Amount { get; set; }

    public long OrderCode { get; set; }

    public string Description { get; set; }

    public Guid PaymentMethodId { get; set; }

    public Guid? WithdrawalRequestId { get; set; }

    public Guid? OrderId { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public WithdrawalRequest? WithdrawalRequest { get; set; }

    public Order? Order { get; set; }

    public TransactionType TransactionType { get; set; }
}