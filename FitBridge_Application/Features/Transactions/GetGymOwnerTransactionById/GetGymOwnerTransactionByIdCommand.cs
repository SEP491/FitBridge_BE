using System;
using FitBridge_Application.Dtos.Transactions;
using MediatR;

namespace FitBridge_Application.Features.Transactions.GetGymOwnerTransactionById;

public class GetGymOwnerTransactionByIdCommand : IRequest<MerchantTransactionDetailDto>
{
    public Guid TransactionId { get; set; }
}
