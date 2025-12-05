using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Specifications.Dashboards.GetRevenueDetail;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Specifications.Dashboards.GetOrderItemForRevenueDetail
{
    public class GetOrderItemForRevenueDetailSpec : BaseSpecification<OrderItem>
    {
        public GetOrderItemForRevenueDetailSpec(Guid userId, string userRole, GetRevenueDetailParams parameters) : base(x =>
            // Filter by user - either FreelancePT or GymOwner
            ((x.FreelancePTPackage != null && x.FreelancePTPackageId == userId) ||
            (x.GymPt != null && x.GymPt.GymOwnerId == userId))
            && x.Transactions.Any(t => t.Status == TransactionStatus.Success)
            // Filter by date range - From date
            && (!parameters.From.HasValue || x.CreatedAt >= parameters.From.Value)
            // Filter by date range - To date (inclusive, end of day)
            && (!parameters.To.HasValue || x.CreatedAt <= parameters.To.Value.Date.AddDays(1).AddTicks(-1))
            // Search by gym course name or freelance PT package name
            && (string.IsNullOrEmpty(parameters.SearchTerm)
                || (x.GymCourse != null && x.GymCourse.Name.ToLower().Contains(parameters.SearchTerm.ToLower()))
                || (x.FreelancePTPackage != null && x.FreelancePTPackage.Name.ToLower().Contains(parameters.SearchTerm.ToLower()))))
        {
            AddInclude(x => x.Order.Coupon);
            AddInclude(x => x.Order.Account);
            AddInclude("Order.Coupon");

            if (userRole == ProjectConstant.UserRoles.FreelancePT)
            {
                AddInclude(x => x.FreelancePTPackage);
            }
            else if (userRole == ProjectConstant.UserRoles.GymOwner)
            {
                AddInclude(x => x.GymPt);
                AddInclude(x => x.GymCourse);
            }

            // Default ordering by order item id descending
            AddOrderByDesc(x => x.Id);

            if (parameters.DoApplyPaging)
            {
                AddPaging((parameters.Page - 1) * parameters.Size, parameters.Size);
            }
            else
            {
                parameters.Size = -1;
                parameters.Page = -1;
            }
        }
    }
}
