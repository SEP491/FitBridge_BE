using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Notifications.GetNotificationsByUserId;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Notifications.GetUnreadCount
{
    internal class GetUnreadNotificationCountQueryHandler(
        IHttpContextAccessor httpContextAccessor,
        IUserUtil userUtil,
        IUnitOfWork unitOfWork) : IRequestHandler<GetUnreadNotificationCountQuery, UnreadNotificationCountDto>
    {
        public async Task<UnreadNotificationCountDto> Handle(GetUnreadNotificationCountQuery request, CancellationToken cancellationToken)
        {
            var accountId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                ?? throw new NotFoundException(nameof(ApplicationUser));

            var spec = new GetNotificationsByUserIdSpecification(accountId, onlyUnread: true);
            var count = await unitOfWork.Repository<Notification>().CountAsync(spec);

            return new UnreadNotificationCountDto { Count = count };
        }
    }
}