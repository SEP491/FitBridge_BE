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
        [HttpPost]
        public async Task<IActionResult> TestNotification([FromBody] Message message)
        {
            await notificationConnectionManager.AddConnectionAsync("0199512e-2c80-7cd9-a843-71f95f30fa71", "connectionId1");
            //add to connection manager to check exisst or mayb not
            var uid = Guid.Parse("0199512e-2c80-7cd9-a843-71f95f30fa71");
            await notificationService.NotifyUsers(new NotificationMessage(
                EnumContentType.NewMessage,
                [uid],
                new NewMessageModel(message.Body, message.Title),
                JsonSerializer.Serialize(new { userId = uid.ToString() })));
            return Ok(new { Message = "Notification sent successfully." });
        }

        public record Message(string Body, string Title);
    }
}