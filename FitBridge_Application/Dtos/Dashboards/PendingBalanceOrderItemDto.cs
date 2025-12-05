namespace FitBridge_Application.Dtos.Dashboards
{
    public class PendingBalanceOrderItemDto
    {
        public Guid OrderItemId { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal SubTotal { get; set; }

        public double? CouponDiscountPercent { get; set; }

        public Guid? CouponId { get; set; }

        public string? CouponCode { get; set; } = string.Empty;

        public Guid CustomerId { get; set; }

        public string CustomerFullName { get; set; } = string.Empty;

        public DateOnly? PlannedDistributionDate { get; set; }

        public string CourseName { get; set; } = string.Empty; // this includes free-pt and gym courses

        public Guid CourseId { get; set; }

        public decimal TotalProfit { get; set; }
    }
}