using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Accounts.Customers;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Accounts.GetAllCustomersForAdmin;
using MediatR;
using AutoMapper;
using FitBridge_Application.Interfaces.Services;

namespace FitBridge_Application.Features.Accounts.GetAllCustomersForAdmin;

public class GetAllCustomersForAdminQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IApplicationUserService _applicationUserService) : IRequestHandler<GetAllCustomersForAdminQuery, PagingResultDto<GetAllCustomersForAdminDto>>
{
    public async Task<PagingResultDto<GetAllCustomersForAdminDto>> Handle(GetAllCustomersForAdminQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetAllCustomersForAdminSpec(request.Params);
        var customers = await _applicationUserService.GetAllUserWithSpecProjectedAsync<GetAllCustomersForAdminDto>(spec, _mapper.ConfigurationProvider);
        var totalItems = await _applicationUserService.CountAsync(spec);
        return new PagingResultDto<GetAllCustomersForAdminDto>(totalItems, customers);
    }
}
