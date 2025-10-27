using FitBridge_Domain.Entities.Reports;

namespace FitBridge_Application.Specifications.Reports.GetReportById
{
    public class GetReportByIdSpec : BaseSpecification<ReportCases>
    {
        public GetReportByIdSpec(Guid reportId) : base(x => x.Id == reportId)
        {
    // Include related entities
            AddInclude(x => x.Reporter);
  AddInclude(x => x.ReportedUser);
   AddInclude(x => x.OrderItem);
 }
    }
}
