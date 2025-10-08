using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Notifications.GetNotificationsByUserId;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Notifications.MarkAllAsRead
{
    internal class MarkAllNotificationsAsReadCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IUserUtil userUtil,
        IUnitOfWork unitOfWork) : IRequestHandler<MarkAllNotificationsAsReadCommand>
    {
        public async Task Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
        {
            var accountId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                ?? throw new NotFoundException(nameof(ApplicationUser));

            var spec = new GetNotificationsByUserIdSpecification(
                accountId, 
                new GetNotificationsByUserIdParams { DoApplyPaging = false });
            
            var notifications = await unitOfWork.Repository<Notification>()
                .GetAllWithSpecificationAsync(spec, asNoTracking: false);

            var now = DateTime.UtcNow;
            foreach (var notification in notifications)
            {
                if (notification.ReadAt == null)
                {
                    notification.ReadAt = now;
                    notification.UpdatedAt = now;
                    unitOfWork.Repository<Notification>().Update(notification);
                }
            }

            await unitOfWork.CommitAsync();
        }
    }
}