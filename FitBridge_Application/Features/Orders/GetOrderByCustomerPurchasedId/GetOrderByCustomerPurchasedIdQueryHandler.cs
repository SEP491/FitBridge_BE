using System;
using AutoMapper;
using FitBridge_Application.Dtos.Orders;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Orders.GetOrderByCustomerPurchasedId;

public class GetOrderByCustomerPurchasedIdQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetOrderByCustomerPurchasedIdQuery, OrderResponseDto>
{
    public async Task<OrderResponseDto> Handle(GetOrderByCustomerPurchasedIdQuery request, CancellationToken cancellationToken)
    {
        var customerPurchasedEntity = await _unitOfWork.Repository<CustomerPurchased>().GetByIdAsync(request.CustomerPurchasedId, false, new List<string> { "OrderItems",
        "OrderItems.Order" });
        if (customerPurchasedEntity == null)
        {
            throw new NotFoundException("Customer purchased not found");
        }
        return _mapper.Map<OrderResponseDto>(customerPurchasedEntity.OrderItems.OrderByDescending(x => x.CreatedAt).First().Order);
    }
}
