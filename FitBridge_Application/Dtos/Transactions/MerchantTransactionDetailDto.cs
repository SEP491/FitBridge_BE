using System;

using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Dtos.Transactions;

public class MerchantTransactionDetailDto
{
    public Guid TransactionId { get; set; }
    public Guid? OrderItemId { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerAvatarUrl { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal? TotalPaidAmount { get; set; }
    public decimal? ProfitAmount { get; set; }
    public long OrderCode { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string PaymentMethod { get; set; }
}
