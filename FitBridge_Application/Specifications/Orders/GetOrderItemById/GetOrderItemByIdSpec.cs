using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.Orders.GetOrderItemById
{
    public class GetOrderItemByIdSpec : BaseSpecification<OrderItem>
    {
        public GetOrderItemByIdSpec(
            Guid orderItemId,
            bool isIncludeProduct = false,
            bool isIncludeGymCourse = false,
            bool isIncludeFreelancePackage = false,
            bool isIncludeCoupon = false,
            bool isIncludeTransaction = false) : base(x => x.Id == orderItemId)
        {
            AddInclude(x => x.Order);
            AddInclude(x => x.Order.Account);

            if (isIncludeTransaction)
            {
                AddInclude(x => x.Transactions);
            }
            if (isIncludeCoupon)
            {
                AddInclude(x => x.Order.Coupon);
            }
            if (isIncludeFreelancePackage)
            {
                AddInclude(x => x.FreelancePTPackage);
            }
            if (isIncludeGymCourse)
            {
                AddInclude(x => x.GymCourse);
            }
            if (isIncludeProduct)
            {
                AddInclude(x => x.ProductDetail);
            }
        }
    }
}