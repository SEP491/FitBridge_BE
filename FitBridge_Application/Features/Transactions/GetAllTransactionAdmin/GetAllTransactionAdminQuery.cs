using System;
using MediatR;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Transactions;
using FitBridge_Application.Specifications.Transactions.GetAllTransactionAdmin;
namespace FitBridge_Application.Features.Transactions.GetAllTransactionAdmin;

public class GetAllTransactionAdminQuery : IRequest<PagingResultDto<GetAllTransactionAdminDto>>
{
    public GetAllTransactionAdminParams Parameters { get; set; }
}
