using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Reports;
using FitBridge_Domain.Enums.Reports;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Reports.UpdateReportStatus
{
    internal class UpdateReportStatusCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<UpdateReportStatusCommand>
    {
        public async Task Handle(UpdateReportStatusCommand request, CancellationToken cancellationToken)
        {
            var existingReport = await unitOfWork.Repository<ReportCases>().GetByIdAsync(request.ReportId, asNoTracking: false)
                ?? throw new NotFoundException(nameof(ReportCases));

            existingReport.Status = request.Status;
            if (request.Status == ReportCaseStatus.Pending)
            {
                existingReport.IsPayoutPaused = true;
            }
            else if (request.Status == ReportCaseStatus.Resolved)
            {
                existingReport.ResolvedAt = DateTime.UtcNow;
                existingReport.IsPayoutPaused = false;
            }
            else if (request.Status == ReportCaseStatus.FraudConfirmed)
            {
                existingReport.ResolvedAt = DateTime.UtcNow;
                existingReport.Note = request.Note;
            }

            unitOfWork.Repository<ReportCases>().Update(existingReport);
            await unitOfWork.CommitAsync();
        }
    }
}