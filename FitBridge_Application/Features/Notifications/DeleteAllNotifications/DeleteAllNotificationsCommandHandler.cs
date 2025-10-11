using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Notifications.GetNotificationsByUserId;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Notifications.DeleteAllNotifications
{
    internal class DeleteAllNotificationsCommandHandler(
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IUserUtil userUtil) : IRequestHandler<DeleteAllNotificationsCommand>
    {
        public async Task Handle(DeleteAllNotificationsCommand request, CancellationToken cancellationToken)
        {
            var accountId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                ?? throw new NotFoundException(nameof(ApplicationUser));
            var spec = new GetNotificationsByUserIdSpecification(accountId);
            var notifications = await unitOfWork.Repository<Notification>()
                .GetAllWithSpecificationAsync(spec);

            foreach (var notification in notifications)
            {
                unitOfWork.Repository<Notification>().SoftDelete(notification);
            }

            await unitOfWork.CommitAsync();
        }
    }
}