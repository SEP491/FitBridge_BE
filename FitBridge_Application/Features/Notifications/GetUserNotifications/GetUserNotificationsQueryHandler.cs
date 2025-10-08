using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Notifications.GetNotificationsByUserId;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Notifications.GetUserNotifications
{
    internal class GetUserNotificationsQueryHandler(
        IHttpContextAccessor httpContextAccessor,
        IUserUtil userUtil,
        IUnitOfWork unitOfWork,
        IMapper mapper) :
        IRequestHandler<GetUserNotificationsQuery, PagingResultDto<NotificationDto>>
    {
        public async Task<PagingResultDto<NotificationDto>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
        {
            var accountId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                ?? throw new NotFoundException(nameof(ApplicationUser));

            var spec = new GetNotificationsByUserIdSpecification(accountId, request.Parameters);
            var notifications = await unitOfWork.Repository<Notification>()
                .GetAllWithSpecificationProjectedAsync<NotificationDto>(spec, mapper.ConfigurationProvider);

            var totalCount = await unitOfWork.Repository<Notification>().CountAsync(spec);

            return new PagingResultDto<NotificationDto>(totalCount, notifications);
        }
    }
}