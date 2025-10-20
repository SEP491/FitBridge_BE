using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Dtos.Templates;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Services.Notifications;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Quartz.Impl.AdoJobStore;

namespace FitBridge_Application.Features.Payments.RejectWithdrawalRequest
{
    internal class RejectWithdrawalRequestCommandHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IUserUtil userUtil,
        IApplicationUserService applicationUserService,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<RejectWithdrawalRequestCommand>
    {
        public async Task Handle(RejectWithdrawalRequestCommand request, CancellationToken cancellationToken)
        {
            var accountId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                ?? throw new UnauthorizedAccessException("User is not authenticated");

            var userRole = userUtil.GetUserRole(httpContextAccessor.HttpContext)
                ?? throw new UnauthorizedAccessException("User role not found");

            var withdrawalRequest = await unitOfWork.Repository<WithdrawalRequest>()
                .GetByIdAsync(request.WithdrawalRequestId, asNoTracking: false)
                ?? throw new NotFoundException(nameof(WithdrawalRequest));

            if (withdrawalRequest.Status != WithdrawalRequestStatus.Pending)
            {
                throw new InvalidDataException($"Cannot reject withdrawal request with status: " +
                    $"{withdrawalRequest.Status}. Only pending requests can be rejected.");
            }

            var newStatus = GetRequestStatus(withdrawalRequest, userRole, accountId);

            UpdateWithdrawalRequestStatus(withdrawalRequest, newStatus, request.Reason);

            await unitOfWork.CommitAsync();

            await SendNotification(newStatus, withdrawalRequest, request.Reason);
        }

        private WithdrawalRequestStatus GetRequestStatus(WithdrawalRequest withdrawalRequest,
            string userRole, Guid accountId)
        {
            WithdrawalRequestStatus newStatus;
            if (userRole.Contains(ProjectConstant.UserRoles.Admin))
            {
                newStatus = WithdrawalRequestStatus.Rejected;
            }
            else if (withdrawalRequest.AccountId == accountId)
            {
                newStatus = WithdrawalRequestStatus.UserDisapproved;
            }
            else
            {
                throw new UnauthorizedAccessException("You are not authorized to reject this withdrawal request.");
            }
            return newStatus;
        }

        private void UpdateWithdrawalRequestStatus(WithdrawalRequest withdrawalRequest,
            WithdrawalRequestStatus newStatus, string reason)
        {
            withdrawalRequest.Status = newStatus;
            withdrawalRequest.Reason = reason;
            withdrawalRequest.UpdatedAt = DateTime.UtcNow;

            unitOfWork.Repository<WithdrawalRequest>().Update(withdrawalRequest);
        }

        private async Task SendNotification(
            WithdrawalRequestStatus withdrawalRequestStatus,
            WithdrawalRequest withdrawalRequest,
            string rejectReason)
        {
            string rejectorName = userUtil.GetUserFullName(httpContextAccessor.HttpContext)
                        ?? throw new NotFoundException("Full name claim");

            List<Guid> toSendAccounts = [withdrawalRequest.AccountId]; // default to UserDisapproved
            if (withdrawalRequestStatus == WithdrawalRequestStatus.UserDisapproved)
            { // notify all admins
                toSendAccounts = await applicationUserService.GetUsersByRoleAsync(
                    ProjectConstant.UserRoles.Admin)
                    .ContinueWith(task => task.Result.Select(user => user.Id).ToList());
            }

            var model = new WithdrawalRequestAdminRejectedModel
            {
                BodyAmount = withdrawalRequest.Amount,
                BodyReason = rejectReason,
                BodyRejectedBy = rejectorName,
            };

            var message = new NotificationMessage(
                EnumContentType.WithdrawalRequestAdminRejected,
                toSendAccounts,
                model);

            await notificationService.NotifyUsers(message);
        }
    }
}