using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.Orders.GetOrderItemById
{
    public class GetOrderItemByIdSpec : BaseSpecification<OrderItem>
    {
        public GetOrderItemByIdSpec(
            Guid orderItemId,
            bool isIncludeProduct = false,
            bool isIncludeGymCourse = false,
            bool isIncludeFreelancePackage = false) : base(x => x.Id == orderItemId)
        {
            AddInclude(x => x.Order);
            AddInclude(x => x.Order.Account);
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