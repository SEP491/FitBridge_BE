using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Dtos.Templates;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services.Notifications;
using FitBridge_Domain.Entities.Reports;
using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Domain.Enums.Reports;
using FitBridge_Domain.Exceptions;
using MediatR;
using System.Text.Json;

namespace FitBridge_Application.Features.Reports.UpdateReportStatus
{
    internal class UpdateReportStatusCommandHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService) : IRequestHandler<UpdateReportStatusCommand>
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

            await SendNotificationToReporter(existingReport);
        }

        private async Task SendNotificationToReporter(ReportCases report)
        {
            var statusMessage = report.Status switch
            {
                ReportCaseStatus.Pending => "Pending Review",
                ReportCaseStatus.Processing => "Under Investigation",
                ReportCaseStatus.Resolved => "Resolved",
                ReportCaseStatus.FraudConfirmed => "Fraud Confirmed",
                _ => report.Status.ToString()
            };

            var model = new ReportStatusUpdatedModel
            {
                TitleReportTitle = report.Title,
                BodyReportTitle = report.Title,
                BodyStatus = statusMessage,
                BodyNote = report.Note ?? "No additional notes provided."
            };

            var notificationMessage = new NotificationMessage(
                EnumContentType.ReportStatusUpdated,
                [report.ReporterId],
                model,
                JsonSerializer.Serialize(new { report.Id }));

            await notificationService.NotifyUsers(notificationMessage);
        }
    }
}