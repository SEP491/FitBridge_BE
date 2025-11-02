using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Accounts.Customers;
using FitBridge_Application.Specifications.Accounts.GetAllCustomersForAdmin;
using MediatR;

namespace FitBridge_Application.Features.Accounts.GetAllCustomersForAdmin;

public class GetAllCustomersForAdminQuery : IRequest<PagingResultDto<GetAllCustomersForAdminDto>>
{
    public GetAllCustomersForAdminParams Params { get; set; }
}
