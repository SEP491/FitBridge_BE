using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Domain.Entities.Orders;

public class Transaction : BaseEntity
{
    public TransactionStatus Status { get; set; }
    public Guid? OrderItemId { get; set; }
    public decimal Amount { get; set; }
    public long OrderCode { get; set; }

    public string Description { get; set; }

    public Guid PaymentMethodId { get; set; }

    public Guid? WithdrawalRequestId { get; set; }

    public Guid? OrderId { get; set; }
    public decimal? ProfitAmount { get; set; }
    public Guid? WalletId { get; set; }

    public PaymentMethod PaymentMethod { get; set; }
    public OrderItem? OrderItem { get; set; }
    public WithdrawalRequest? WithdrawalRequest { get; set; }
    public Order? Order { get; set; }
    public Wallet? Wallet { get; set; }
    public TransactionType TransactionType { get; set; }
}