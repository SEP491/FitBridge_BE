using FitBridge_Domain.Entities.Reports;

namespace FitBridge_Application.Specifications.Reports.GetCustomerReports
{
    public class GetCustomerReportsSpec : BaseSpecification<ReportCases>
    {
        public GetCustomerReportsSpec(GetCustomerReportsParams parameters, Guid customerId) : base(x =>
             x.ReporterId == customerId &&
            (string.IsNullOrEmpty(parameters.SearchTerm) ||
            x.Title.ToLower().Contains(parameters.SearchTerm.ToLower()) ||
            (x.Description != null && x.Description.ToLower().Contains(parameters.SearchTerm.ToLower()))))
        {
            // Include related entities
            AddInclude(x => x.Reporter);
            AddInclude(x => x.ReportedUser);
            AddInclude(x => x.OrderItem);

            // Apply sorting
            if (parameters.SortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
            {
                if (parameters.SortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    AddOrderByDesc(x => x.CreatedAt);
                }
                else
                {
                    AddOrderBy(x => x.CreatedAt);
                }
            }
            else if (parameters.SortBy.Equals("Status", StringComparison.OrdinalIgnoreCase))
            {
                if (parameters.SortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    AddOrderByDesc(x => x.Status);
                }
                else
                {
                    AddOrderBy(x => x.Status);
                }
            }
            else
            {
                // Default sorting by CreatedAt descending
                AddOrderByDesc(x => x.CreatedAt);
            }

            // Apply paging
            if (parameters.DoApplyPaging)
            {
                AddPaging(parameters.Size * (parameters.Page - 1), parameters.Size);
            }
        }
    }
}