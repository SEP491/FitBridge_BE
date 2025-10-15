using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Dtos.Templates;
using FitBridge_Application.Interfaces.Services.Notifications;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Domain.Enums.MessageAndReview;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Coupons.GiftCoupon
{
    internal class GiftCouponCommandHandler(
        INotificationService notificationService,
        IHttpContextAccessor httpContextAccessor,
        IUserUtil userUtil) : IRequestHandler<GiftCouponCommand>
    {
        public Task Handle(GiftCouponCommand request, CancellationToken cancellationToken)
        {
            var userName = userUtil.GetUserFullName(httpContextAccessor.HttpContext!);
            var notificationMessage = new NotificationMessage(
                EnumContentType.NewCoupon,
                request.CustomerIds,
                new NewCouponModel(request.CouponCode, userName, request.CouponCode));

            notificationService.NotifyUsers(notificationMessage);

            return Task.CompletedTask;
        }
    }
}