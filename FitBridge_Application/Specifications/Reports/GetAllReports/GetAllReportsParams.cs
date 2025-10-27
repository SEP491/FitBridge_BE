using FitBridge_Application.Specifications;
using FitBridge_Domain.Enums.Reports;

namespace FitBridge_Application.Specifications.Reports.GetAllReports
{
    public class GetAllReportsParams : BaseParams
    {
        public ReportCaseStatus? Status { get; set; }

        public ReportCaseType? ReportType { get; set; }

   public Guid? ReporterId { get; set; }

    public Guid? ReportedUserId { get; set; }
    }
}
