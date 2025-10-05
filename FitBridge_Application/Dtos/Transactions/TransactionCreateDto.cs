using System;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Dtos.Transactions;

public class TransactionCreateDto
{
    public long OrderCode { get; set; }

    public string Description { get; set; }

    public TransactionType Type { get; set; }

    public Guid PaymentMethodId { get; set; }
}