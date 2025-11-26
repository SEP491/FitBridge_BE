using System;

namespace FitBridge_Application.Dtos.Orders;

public class GetManageProductOrder(SummaryProductOrderDto summaryProductOrder, PagingResultDto<GetAllProductOrderResponseDto> productOrders)
{
    public SummaryProductOrderDto SummaryProductOrder { get; private set; } = summaryProductOrder;
    public PagingResultDto<GetAllProductOrderResponseDto> ProductOrders { get; private set; } = productOrders;
}
