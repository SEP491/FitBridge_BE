using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Dtos.Reports;
using FitBridge_Application.Dtos.Templates;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Services.Notifications;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Orders.GetOrderItemById;
using FitBridge_Application.Specifications.Reports.GetReportByOrderItemId;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Entities.Reports;
using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Domain.Enums.Reports;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace FitBridge_Application.Features.Reports.CreateReport
{
    internal class CreateReportCommandHandler(
        IUnitOfWork unitOfWork,
        IUserUtil userUtil,
        IHttpContextAccessor httpContextAccessor,
        IApplicationUserService applicationUserService,
        INotificationService notificationService) : IRequestHandler<CreateReportCommand, CreateReportResponseDto>
    {
        public async Task<CreateReportResponseDto> Handle(CreateReportCommand request, CancellationToken cancellationToken)
        {
            var reporterId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                ?? throw new NotFoundException(nameof(ApplicationUser));

            var report = await GetExistingReport(request.ReportedItemId, reporterId);
            if (report != null)
            {
                throw new DataValidationFailedException("An ongoing report for this item already exists.");
            }

            var (reportedId, reportType) = await GetReportedUserIdAndReportTypeAsync(request.ReportedItemId);

            var newReport = new ReportCases
            {
                ReporterId = reporterId,
                ReportedUserId = reportedId,
                OrderItemId = request.ReportedItemId,
                ReportType = reportType,
                Status = ReportCaseStatus.Pending,
                ImageUrls = request.ImageUrls,
                Title = request.Title,
                Description = request.Description,
            };

            unitOfWork.Repository<ReportCases>().Insert(newReport);
            await unitOfWork.CommitAsync();

            await SendNotificationToAdmins(request.Title, reportType, newReport);

            return new CreateReportResponseDto
            {
                ReportId = newReport.Id,
            };
        }

        private async Task<ReportCases?> GetExistingReport(Guid reportedItemId, Guid reporterId)
        {
            var spec = new GetReportByOrderItemIdSpec(reportedItemId, reporterId, isGetOngoingOnly: true);
            return await unitOfWork.Repository<ReportCases>()
                .GetBySpecificationAsync(spec);
        }

        private async Task<(Guid?, ReportCaseType)> GetReportedUserIdAndReportTypeAsync(Guid reportedItemId)
        {
            var spec = new GetOrderItemByIdSpec(
                reportedItemId,
                isIncludeProduct: true,
                isIncludeFreelancePackage: true,
                isIncludeGymCourse: true);
            var orderItem = await unitOfWork.Repository<OrderItem>()
                .GetBySpecificationAsync(spec)
                ?? throw new NotFoundException(nameof(OrderItem));

            if (orderItem.FreelancePTPackage != null)
            {
                return (orderItem.FreelancePTPackage.PtId, ReportCaseType.FreelancePtReport);
            }
            else if (orderItem.GymCourse != null)
            {
                return (orderItem.GymCourse.GymOwnerId, ReportCaseType.GymCourseReport);
            }
            else if (orderItem.ProductDetail != null)
            {
                return (null, ReportCaseType.ProductReport);
            }
            else
            {
                throw new DataValidationFailedException("Invalid report case type.");
            }
        }

        private async Task SendNotificationToAdmins(string reportTitle, ReportCaseType reportType, ReportCases newReport)
        {
            var admins = await applicationUserService.GetUsersByRoleAsync(
                ProjectConstant.UserRoles.Admin);
            var reporterName = userUtil.GetUserFullName(httpContextAccessor.HttpContext);

            var model = new NewReportModel
            {
                TitleReporterName = reporterName ?? "Anonymous",
                BodyReporterName = reporterName ?? "Anonymous",
                BodyReportTitle = reportTitle,
                BodyReportType = reportType.ToString()
            };

            var notificationMessage = new NotificationMessage(
                EnumContentType.NewReport,
                admins.Select(admin => admin.Id).ToList(),
                model,
                JsonSerializer.Serialize(new { newReport.Id }));

            await notificationService.NotifyUsers(notificationMessage);
        }
    }
}