using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Specifications.Orders.GetAllProductOrders;

public class GetAllProductOrdersSpec : BaseSpecification<Order>
{
    public GetAllProductOrdersSpec(GetAllProductOrdersParams parameters) : base(x =>
        x.Transactions.Any(t => t.TransactionType == TransactionType.ProductOrder)
        &&
        (!parameters.CustomerId.HasValue || x.AccountId == parameters.CustomerId.Value) &&
        (!parameters.OrderId.HasValue || x.Id == parameters.OrderId.Value) &&
        (!parameters.Status.HasValue || x.Status == parameters.Status.Value) &&
        (!parameters.FromTime.HasValue || x.CreatedAt >= parameters.FromTime.Value) &&
        (!parameters.ToTime.HasValue || x.CreatedAt <= parameters.ToTime.Value) &&
        (parameters.ShippingTrackingId == null || x.ShippingTrackingId == parameters.ShippingTrackingId)
    )
    {
        AddInclude(x => x.Account);
        AddInclude(x => x.Address);
        AddInclude(x => x.Coupon);
        AddInclude(x => x.OrderItems);
        AddInclude("OrderItems.ProductDetail");
        AddInclude(x => x.OrderStatusHistories);
        if(parameters.SortOrder == "asc")
        {
            AddOrderBy(x => x.CreatedAt);
        }
        else
        {
            AddOrderByDesc(x => x.CreatedAt);
        }
        if (parameters.DoApplyPaging)
        {
            AddPaging(parameters.Size * (parameters.Page - 1), parameters.Size);
        }
        else
        {
            parameters.Size = -1;
            parameters.Page = -1;
        }
    }
}
