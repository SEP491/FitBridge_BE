namespace FitBridge_Domain.Enums.Orders
{
    public enum OrderStatus
    {
        Created,

        Pending,

        Processing,
        Shipping,

        Arrived,

        Cancelled,
        Finished,
        Assigning,
        Accepted,
        CustomerNotReceived,
        InReturn,
        Returned
    }
}