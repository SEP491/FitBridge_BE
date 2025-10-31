using System;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Dtos.Transactions;

public class GetAllMerchantTransactionDto
{
    public Guid TransactionId { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal? TotalPaidAmount { get; set; }
    public decimal? ProfitAmount { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerAvatarUrl { get; set; }
    public long OrderCode { get; set; }
}
 