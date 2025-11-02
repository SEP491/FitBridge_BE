using System;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Dtos.Transactions;

public class GetAllTransactionAdminDto
{
    public Guid TransactionId { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal? Amount { get; set; }
    public string OrderCode { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string PaymentMethod { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerAvatarUrl { get; set; }
    public decimal? ProfitAmount { get; set; }
    public Guid? OrderId { get; set; }
    public Guid? WalletId { get; set; }
    public Guid? WithdrawalRequestId { get; set; }
}
