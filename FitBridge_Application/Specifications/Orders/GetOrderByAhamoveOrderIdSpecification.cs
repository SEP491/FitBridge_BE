using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.Orders;

public class GetOrderByAhamoveOrderIdSpecification : BaseSpecification<Order>
{
    public GetOrderByAhamoveOrderIdSpecification(string shippingTrackingId) : base(o => o.ShippingTrackingId == shippingTrackingId)
    {
        AddInclude(o => o.Transactions);
        AddInclude(o => o.OrderItems);
        AddInclude(o => o.OrderStatusHistories);
        AddInclude("Transactions.PaymentMethod");
    }
}

