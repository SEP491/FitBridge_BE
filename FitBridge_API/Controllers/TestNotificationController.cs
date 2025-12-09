using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Dtos.Templates;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services.Notifications;
using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Infrastructure.Services.Notifications.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FitBridge_API.Controllers
{
    public class TestNotificationController(
        INotificationService notificationService,
        NotificationConnectionManager notificationConnectionManager) : _BaseApiController
    {
        public record TestNotiDto(string Body, string Title, Guid userId);

        [HttpPost]
        public async Task<IActionResult> TestNotification([FromBody] TestNotiDto message)
        {
            await notificationConnectionManager.AddConnectionAsync(message.userId.ToString(), "connectionId1");
            var uid = message.userId;
            await notificationService.NotifyUsers(new NotificationMessage(
                EnumContentType.NewPaymentRequest,
                [uid],
                new NewPaymentRequestModel
                {
                    BodyAmmount = 100,
                    BodyRequesterName = "John Doe",
                    TitleRequesterName
                     = "John Doe1"
                },
                JsonSerializer.Serialize(new { userId = uid.ToString() })));
            return Ok(new { Message = "Notification sent successfully." });
        }

        public record Message(string Body, string Title);
    }
}