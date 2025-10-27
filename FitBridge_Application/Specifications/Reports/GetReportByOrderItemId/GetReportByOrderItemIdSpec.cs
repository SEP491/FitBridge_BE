using FitBridge_Domain.Entities.Reports;
using FitBridge_Domain.Enums.Reports;

namespace FitBridge_Application.Specifications.Reports.GetReportByOrderItemId
{
    public class GetReportByOrderItemIdSpec : BaseSpecification<ReportCases>
    {
        public GetReportByOrderItemIdSpec(
            Guid reportedItemId,
            Guid reporterId,
            bool isGetOngoingOnly = false) : base(x =>
            x.OrderItemId == reportedItemId
            && x.ReporterId == reporterId
            && (!isGetOngoingOnly || (isGetOngoingOnly && x.Status != (ReportCaseStatus.Resolved & ReportCaseStatus.FraudConfirmed))))
        {
        }
    }
}