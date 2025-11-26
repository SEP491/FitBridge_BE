using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Orders;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Orders.GetAllProductOrders;
using MediatR;
using AutoMapper;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Features.Orders.GetAllProductOrder;

public class GetAllProductOrdersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetAllProductOrdersQuery, GetManageProductOrder>
{
    public async Task<GetManageProductOrder> Handle(GetAllProductOrdersQuery request, CancellationToken cancellationToken)
    {
        var summaryProductOrder = new SummaryProductOrderDto();
        var spec = new GetAllProductOrdersSpec(request.Params);
        var orders = await unitOfWork.Repository<Order>().GetAllWithSpecificationAsync(spec);
        var totalItems = await unitOfWork.Repository<Order>().CountAsync(spec);
        var dtos = mapper.Map<IReadOnlyList<GetAllProductOrderResponseDto>>(orders);
        var productOrdersResponse = new PagingResultDto<GetAllProductOrderResponseDto>(totalItems, dtos);
        await aggregateSummaryProductOrder(orders.ToList(), summaryProductOrder);
        var getManageProductOrder = new GetManageProductOrder(summaryProductOrder, productOrdersResponse);

        return getManageProductOrder;
    }

    public async Task aggregateSummaryProductOrder(List<Order> orders, SummaryProductOrderDto summaryProductOrder)
    {
        foreach (var order in orders)
        {
            summaryProductOrder.totalProductOrders++;
            switch (order.Status)
            {
                case OrderStatus.Pending:
                    summaryProductOrder.totalPending++;
                    break;
                case OrderStatus.Processing:
                    summaryProductOrder.totalProcessing++;
                    break;
                case OrderStatus.Shipping:
                    summaryProductOrder.totalShipping++;
                    break;
                case OrderStatus.Arrived:
                    summaryProductOrder.totalArrived++;
                    break;
                case OrderStatus.Cancelled:
                    summaryProductOrder.totalCancelled++;
                    break;
                case OrderStatus.Finished:
                    summaryProductOrder.totalFinished++;
                    summaryProductOrder.totalProfit += order.TotalAmount;
                    break;
                case OrderStatus.InReturn:
                    summaryProductOrder.totalInReturn++;
                    break;
                case OrderStatus.Returned:
                    summaryProductOrder.totalReturned++;
                    break;
                case OrderStatus.Created:
                    summaryProductOrder.totalCreated++;
                    break;
                case OrderStatus.Accepted:
                    summaryProductOrder.totalAccepted++;
                    break;
                case OrderStatus.CustomerNotReceived:
                    summaryProductOrder.totalCustomerNotReceived++;
                    break;
                case OrderStatus.Assigning:
                    summaryProductOrder.totalAssigning++;
                    break;
                default:
                    break;
            }
            summaryProductOrder.totalRevenue += order.TotalAmount;
            if (order.Transactions.Count() == 0)
            {
                continue;
            }
            var paymentMethod = await unitOfWork.Repository<PaymentMethod>().GetByIdAsync(order.Transactions.OrderByDescending(o => o.CreatedAt).First().PaymentMethodId);
            if (order.OrderStatusHistories.Any(o => o.Status == OrderStatus.Returned) && paymentMethod.MethodType == MethodType.COD)
            {
                summaryProductOrder.totalProfit -= order.ShippingFeeActualCost.Value;
            }
        }
    }
}
