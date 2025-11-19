using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Orders;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Orders.GetAllProductOrders;
using MediatR;
using AutoMapper;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Features.Orders.GetAllProductOrder;

public class GetAllProductOrdersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetAllProductOrdersQuery, PagingResultDto<GetAllProductOrderResponseDto>>
{
    public async Task<PagingResultDto<GetAllProductOrderResponseDto>> Handle(GetAllProductOrdersQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetAllProductOrdersSpec(request.Params);
        var orders = await unitOfWork.Repository<Order>().GetAllWithSpecificationAsync(spec);
        var totalItems = await unitOfWork.Repository<Order>().CountAsync(spec);
        var dtos = mapper.Map<IReadOnlyList<GetAllProductOrderResponseDto>>(orders);
        return new PagingResultDto<GetAllProductOrderResponseDto>(totalItems, dtos);
    }
}
