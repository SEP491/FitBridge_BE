using FitBridge_Domain.Entities.Reports;

namespace FitBridge_Application.Specifications.Reports.GetAllReports
{
    public class GetAllReportsSpec : BaseSpecification<ReportCases>
    {
        public GetAllReportsSpec(GetAllReportsParams parameters) : base(x =>
            (!parameters.Status.HasValue || x.Status == parameters.Status.Value) &&
        (!parameters.ReportType.HasValue || x.ReportType == parameters.ReportType.Value) &&
        (!parameters.ReporterId.HasValue || x.ReporterId == parameters.ReporterId.Value) &&
            (!parameters.ReportedUserId.HasValue || x.ReportedUserId == parameters.ReportedUserId.Value) &&
      (string.IsNullOrEmpty(parameters.SearchTerm) ||
 x.Title.ToLower().Contains(parameters.SearchTerm.ToLower()) ||
  (x.Description != null && x.Description.ToLower().Contains(parameters.SearchTerm.ToLower())) ||
            x.Reporter.FullName.ToLower().Contains(parameters.SearchTerm.ToLower()) ||
            (x.ReportedUser != null && x.ReportedUser.FullName.ToLower().Contains(parameters.SearchTerm.ToLower()))))
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
        else if (parameters.SortBy.Equals("ReportType", StringComparison.OrdinalIgnoreCase))
  {
      if (parameters.SortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
   {
AddOrderByDesc(x => x.ReportType);
         }
 else
        {
        AddOrderBy(x => x.ReportType);
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
