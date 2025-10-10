using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Notifications;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Enums.Notifications;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Notifications.AddUserDeviceToken
{
    internal class AddUserDeviceTokenCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IUserUtil userUtil,
        IUnitOfWork unitOfWork) : IRequestHandler<AddUserDeviceTokenCommand>
    {
        public async Task Handle(AddUserDeviceTokenCommand request, CancellationToken cancellationToken)
        {
            var accountId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                ?? throw new NotFoundException(nameof(ApplicationUser));

            var newToken = new PushNotificationTokens
            {
                Id = Guid.NewGuid(),
                DeviceToken = request.DeviceToken,
                UserId = accountId,
                Platform = request.Platform,
                IsEnabled = true
            };

            unitOfWork.Repository<PushNotificationTokens>().Insert(newToken);
                await unitOfWork.CommitAsync();
        }
    }
}