using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Transactions;
using FitBridge_Application.Specifications;
using FitBridge_Application.Specifications.Transactions.GetAllGymOwnerTransaction;
using FitBridge_Domain.Entities.Orders;
using MediatR;

namespace FitBridge_Application.Features.Transactions.GetAllGymOwnerTransaction;

public class GetAllGymOwnerTransactionQuery : IRequest<PagingResultDto<GetAllMerchantTransactionDto>>
{
    public GetAllGymOwnerTransactionParams Parameters { get; set; }
}
