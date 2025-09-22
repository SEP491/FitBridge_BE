using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Domain.Entities.Orders;

public class PaymentMethod : BaseEntity
{
    public MethodType MethodType { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}