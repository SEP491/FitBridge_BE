using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Features.Notifications.AddUserDeviceToken;
using FitBridge_Application.Features.Notifications.DeleteNotification;
using FitBridge_Application.Features.Notifications.GetUnreadCount;
using FitBridge_Application.Features.Notifications.GetUserNotifications;
using FitBridge_Application.Features.Notifications.MarkAllAsRead;
using FitBridge_Application.Features.Notifications.MarkNotificationAsRead;
using FitBridge_Application.Specifications.Notifications.GetNotificationsByUserId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers
{
    /// <summary>
    /// Controller for managing notifications, including retrieval, marking as read, deletion, and device token management.
    /// </summary>
    [Authorize]
    public class NotificationsController(IMediator mediator) : _BaseApiController
    {
        /// <summary>
        /// Retrieves a paginated list of notifications for the authenticated user.
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination, including:
        /// <list type="bullet">
        /// <item>
        /// <term>Page</term>
        /// <description>The page number to retrieve (default: 1).</description>
        /// </item>
        /// <item>
        /// <term>Size</term>
        /// <description>The number of items per page (default: 10, max: 20).</description>
        /// </item>
        /// <item>
        /// <term>DoApplyPaging</term>
        /// <description>Whether to apply pagination (default: true).</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>A paginated list of notifications for the authenticated user.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<Pagination<NotificationDto>>))]
        public async Task<ActionResult<Pagination<NotificationDto>>> GetUserNotifications([FromQuery] GetNotificationsByUserIdParams parameters)
        {
            var response = await mediator.Send(new GetUserNotificationsQuery(parameters));

            var pagedResult = new Pagination<NotificationDto>(
                response.Items,
                response.Total,
                parameters.Page,
                parameters.Size);

            return Ok(
                new BaseResponse<Pagination<NotificationDto>>(
                    StatusCodes.Status200OK.ToString(),
                    "Get user notifications success",
                    pagedResult));
        }

        /// <summary>
        /// Retrieves the count of unread notifications for the authenticated user.
        /// </summary>
        /// <returns>The number of unread notifications wrapped in a DTO.</returns>
        [HttpGet("unread-count")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<UnreadNotificationCountDto>))]
        public async Task<ActionResult<UnreadNotificationCountDto>> GetUnreadNotificationCount()
        {
            var result = await mediator.Send(new GetUnreadNotificationCountQuery());

            return Ok(
                new BaseResponse<UnreadNotificationCountDto>(
                    StatusCodes.Status200OK.ToString(),
                    "Unread notification count retrieved successfully",
                    result));
        }

        /// <summary>
        /// Marks a specific notification as read.
        /// </summary>
        /// <param name="notificationId">The unique identifier of the notification to mark as read.</param>
        /// <returns>A success response if the operation is successful.</returns>
        [HttpPut("{notificationId}/read")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<EmptyResult>))]
        public async Task<IActionResult> MarkNotificationAsRead([FromRoute] Guid notificationId)
        {
            await mediator.Send(new MarkNotificationAsReadCommand { Id = notificationId });

            return Ok(
                new BaseResponse<EmptyResult>(
                    StatusCodes.Status200OK.ToString(),
                    "Notification marked as read successfully",
                    Empty));
        }

        /// <summary>
        /// Marks all notifications for the authenticated user as read.
        /// </summary>
        /// <returns>A success response if the operation is successful.</returns>
        [HttpPut("read-all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<EmptyResult>))]
        public async Task<IActionResult> MarkAllNotificationsAsRead()
        {
            await mediator.Send(new MarkAllNotificationsAsReadCommand());

            return Ok(
                new BaseResponse<EmptyResult>(
                    StatusCodes.Status200OK.ToString(),
                    "All notifications marked as read successfully",
                    Empty));
        }

        /// <summary>
        /// Deletes a specific notification.
        /// </summary>
        /// <param name="notificationId">The unique identifier of the notification to delete.</param>
        /// <returns>A success response if the deletion is successful.</returns>
        [HttpDelete("{notificationId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<EmptyResult>))]
        public async Task<IActionResult> DeleteNotification([FromRoute] Guid notificationId)
        {
            await mediator.Send(new DeleteNotificationCommand { Id = notificationId });

            return Ok(
                new BaseResponse<EmptyResult>(
                    StatusCodes.Status200OK.ToString(),
                    "Notification deleted successfully",
                    Empty));
        }

        /// <summary>
        /// Registers a device token for push notifications for the authenticated user.
        /// </summary>
        /// <param name="command">The command containing the device token information:
        /// <list type="bullet">
        /// <item>
        /// <term>DeviceToken</term>
        /// <description>The Firebase Cloud Messaging (FCM) device token string used to send push notifications to the user's device. This token is unique to each device and app instance.</description>
        /// <term>Platform</term>
        /// <description>The device's platform</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>A success response if the registration is successful.</returns>
        [HttpPost("device-token")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<EmptyResult>))]
        public async Task<IActionResult> AddUserDeviceToken([FromBody] AddUserDeviceTokenCommand command)
        {
            await mediator.Send(command);

            return Ok(
                new BaseResponse<EmptyResult>(
                    StatusCodes.Status200OK.ToString(),
                    "Device token registered successfully",
                    Empty));
        }
    }
}