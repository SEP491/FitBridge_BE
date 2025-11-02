using System;

using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Transactions;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Transactions.GetAllTransactionAdmin;
using FitBridge_Domain.Entities.Orders;
using MediatR;
using AutoMapper;

namespace FitBridge_Application.Features.Transactions.GetAllTransactionAdmin;

public class GetAllTransactionAdminQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetAllTransactionAdminQuery, PagingResultDto<GetAllTransactionAdminDto>>
{
    public async Task<PagingResultDto<GetAllTransactionAdminDto>> Handle(GetAllTransactionAdminQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetAllTransactionAdminSpec(request.Parameters);
        var transactions = await _unitOfWork.Repository<Transaction>().GetAllWithSpecificationProjectedAsync<GetAllTransactionAdminDto>(spec, _mapper.ConfigurationProvider);
        var totalCount = await _unitOfWork.Repository<Transaction>().CountAsync(spec);
        return new PagingResultDto<GetAllTransactionAdminDto>(totalCount, transactions);
    }
}