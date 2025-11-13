using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Messaging;
using FitBridge_Application.Features.Messaging.CreateConversation;
using FitBridge_Application.Features.Messaging.DeleteMessage;
using FitBridge_Application.Features.Messaging.GetConversations;
using FitBridge_Application.Features.Messaging.GetConversationWithUser;
using FitBridge_Application.Features.Messaging.GetMessages;
using FitBridge_Application.Features.Messaging.GetMessagesInRange;
using FitBridge_Application.Features.Messaging.ReactMessage;
using FitBridge_Application.Features.Messaging.ReadMessages;
using FitBridge_Application.Features.Messaging.SendMessage;
using FitBridge_Application.Features.Messaging.UpdateMessage;
using FitBridge_Application.Specifications.Messaging.GetConversations;
using FitBridge_Application.Specifications.Messaging.GetMessages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers;

/// <summary>
/// Controller for managing real-time messaging, conversations, and message interactions.
/// Provides endpoints for creating conversations, sending/receiving messages, reactions, and message status updates.
/// </summary>
[Authorize]
[ApiController]
public class MessagingController(IMediator _mediator) : _BaseApiController
{
    #region Conversations

    /// <summary>
    /// Retrieves a paginated list of conversations for the authenticated user.
    /// </summary>
    /// <param name="parameters">Query parameters for pagination including:
    /// <list type="bullet">
    /// <item>
    /// <term>Page</term>
    /// <description>The page number to retrieve (default: 1).</description>
    /// </item>
    /// <item>
    /// <term>Size</term>
    /// <description>The number of items per page (default: 20).</description>
    /// </item>
    /// </list>
    /// </param>
    /// <returns>A paginated list of conversations with their details.</returns>
    /// <remarks>
    /// Returns conversation information including:
    /// - Basic info (ID, IsGroup, CustomTitle, UpdatedAt)
    /// - Last message details (Content, Type, MediaType, Sender)
    /// - Read status for the current user
    /// - Conversation members with their roles
    ///
    /// Sample request:
    ///
    ///     GET /api/v1/messaging/conversations?page=1&amp;size=20
    ///
    /// </remarks>
    /// <response code="200">Conversations retrieved successfully</response>
    /// <response code="401">Unauthorized - User not authenticated</response>
    [HttpGet("conversations")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<Pagination<GetConversationsDto>>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetConversations([FromQuery] GetConversationsParam parameters)
    {
        var query = new GetConversationsQuery { Params = parameters };
        var response = await _mediator.Send(query);
        var pagination = ResultWithPagination(response.Items, response.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<GetConversationsDto>>(
            StatusCodes.Status200OK.ToString(),
            "Conversations retrieved successfully",
            pagination));
    }

    /// <summary>
    /// Creates a new conversation with specified members and an initial message.
    /// </summary>
    /// <param name="command">The conversation creation details including:
    /// <list type="bullet">
    /// <item>
    /// <term>IsGroup</term>
    /// <description>Whether this is a group conversation. Must be true if more than 2 members.</description>
    /// </item>
    /// <item>
    /// <term>Members</term>
    /// <description>List of members to include in the conversation (MemberId, MemberName, MemberAvatarUrl).</description>
    /// </item>
    /// <item>
    /// <term>NewMessageContent</term>
    /// <description>The content of the initial message in the conversation.</description>
    /// </item>
    /// <item>
    /// <term>GroupImage</term>
    /// <description>URL of the group image (optional, only for group conversations).</description>
    /// </item>
    /// </list>
    /// </param>
    /// <returns>The ID of the created or existing conversation.</returns>
    /// <remarks>
    /// If a conversation with the exact same members already exists, returns the existing conversation ID.
    /// For non-group conversations, each member sees the other member's name and avatar as the conversation title/image.
    /// For group conversations, all members see the same group name (comma-separated member names) and group image.
    ///
    /// Sample request:
    ///
    ///     POST /api/v1/messaging/conversations
    ///     {
    ///         "isGroup": false,
    ///         "members": [
    ///             {
    ///                 "memberId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///                 "memberName": "John Doe",
    ///                 "memberAvatarUrl": "https://example.com/avatar1.jpg"
    ///             },
    ///             {
    ///                 "memberId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
    ///                 "memberName": "Jane Smith",
    ///                 "memberAvatarUrl": "https://example.com/avatar2.jpg"
    ///             }
    ///         ],
    ///         "newMessageContent": "Hello! Let's discuss the workout plan.",
    ///         "groupImage": null
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Conversation created successfully or existing conversation returned</response>
    /// <response code="400">Invalid request - e.g., more than 2 members without IsGroup flag</response>
    /// <response code="401">Unauthorized - User not authenticated</response>
    /// <response code="404">User not found</response>
    [HttpPost("conversations")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<CreateConversationResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateConversation([FromBody] CreateConversationCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(new BaseResponse<CreateConversationResponse>(
            StatusCodes.Status200OK.ToString(),
            "Conversation created successfully",
            response));
    }

    /// <summary>
    /// Retrieves an existing conversation with a specific user, or null if no conversation exists.
    /// </summary>
    /// <param name="userId">The ID of the user to find a conversation with.</param>
    /// <returns>Conversation details if found, null otherwise.</returns>
    /// <remarks>
    /// Useful for checking if a conversation already exists before creating a new one.
    /// Only works for one-on-one conversations (non-group).
    ///
    /// Sample request:
    ///
    ///     GET /api/v1/messaging/conversations/with-user/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///
    /// </remarks>
    /// <response code="200">Request processed successfully (conversation may be null if not found)</response>
    /// <response code="401">Unauthorized - User not authenticated</response>
    [HttpGet("conversations/with-user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<GetConversationWithUserResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetConversationWithUser([FromRoute] Guid userId)
    {
        var query = new GetConversationWithUserQuery { UserId = userId };
        var response = await _mediator.Send(query);
        return Ok(new BaseResponse<GetConversationWithUserResponse>(
            StatusCodes.Status200OK.ToString(),
            response != null ? "Conversation found" : "No conversation exists with this user",
            response));
    }

    #endregion

    #region Messages

    /// <summary>
    /// Retrieves messages from a specific conversation with pagination.
    /// </summary>
    /// <param name="conversationId">The ID of the conversation to retrieve messages from.</param>
    /// <param name="parameters">Pagination parameters including:
    /// <list type="bullet">
    /// <item>
    /// <term>Page</term>
    /// <description>The page number to retrieve (default: 1).</description>
    /// </item>
    /// <item>
    /// <term>Size</term>
    /// <description>The number of messages per page (default: 20).</description>
    /// </item>
    /// </list>
    /// </param>
    /// <returns>A list of messages with full details including sender info, reactions, and booking requests.</returns>
    /// <remarks>
    /// Returns message information including:
    /// - Message content, type (User/System), and media type (Text/Image/Video/BookingRequest)
    /// - Sender details (ID, name, avatar)
    /// - Message status (Sent/Delivered/Read/Deleted/Edited)
    /// - Delivery status for the current user
    /// - Reply-to message information if applicable
    /// - Reaction emoji if present
    /// - Booking request details if the message contains a booking request
    ///
    /// Sample request:
    ///
    ///     GET /api/v1/messaging/conversations/3fa85f64-5717-4562-b3fc-2c963f66afa6/messages?page=1&amp;size=20
    ///
    /// </remarks>
    /// <response code="200">Messages retrieved successfully</response>
    /// <response code="401">Unauthorized - User not authenticated</response>
    /// <response code="404">User or conversation not found</response>
    [HttpGet("conversations/{conversationId}/messages")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<GetMessagesDto>>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMessages([FromRoute] Guid conversationId, [FromQuery] GetMessagesParam parameters)
    {
        var query = new GetMessagesQuery
        {
            ConversationId = conversationId,
            Params = parameters
        };
        var response = await _mediator.Send(query);
        return Ok(new BaseResponse<IEnumerable<GetMessagesDto>>(
            StatusCodes.Status200OK.ToString(),
            "Messages retrieved successfully",
            response));
    }

    /// <summary>
    /// Retrieves messages in a range between current page and a target message for "jump to message" functionality.
    /// </summary>
    /// <param name="conversationId">The ID of the conversation.</param>
    /// <param name="targetMessageId">The ID of the message to jump to.</param>
    /// <param name="currentPage">The current page number the user is on.</param>
    /// <returns>Messages between the current page and the target message page.</returns>
    /// <remarks>
    /// This endpoint is used to load messages when a user wants to jump to a specific message in the conversation
    /// (e.g., replying to an older message or searching for a specific message).
    /// Returns null if the target message is on or before the current page.
    ///
    /// Sample request:
    ///
    ///     GET /api/v1/messaging/conversations/3fa85f64-5717-4562-b3fc-2c963f66afa6/messages/range?targetMessageId=3fa85f64-5717-4562-b3fc-2c963f66afa7&amp;currentPage=1
    ///
    /// </remarks>
    /// <response code="200">Messages retrieved successfully or null if target is not ahead</response>
    /// <response code="401">Unauthorized - User not authenticated</response>
    /// <response code="404">User or conversation not found</response>
    [HttpGet("conversations/{conversationId}/messages/range")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<GetMessagesDto>>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMessagesInRange(
        [FromRoute] Guid conversationId,
        [FromQuery] Guid targetMessageId,
        [FromQuery] int currentPage = 1)
    {
        var query = new GetMessagesInRangeQuery
        {
            ConversationId = conversationId,
            TargetMessageId = targetMessageId,
            CurrentPage = currentPage
        };
        var response = await _mediator.Send(query);
        return Ok(new BaseResponse<IEnumerable<GetMessagesDto>>(
            StatusCodes.Status200OK.ToString(),
            response != null ? "Messages retrieved successfully" : "Target message is on or before current page",
            response));
    }

    /// <summary>
    /// Sends a new message in a conversation.
    /// </summary>
    /// <param name="command">The message details including:
    /// <list type="bullet">
    /// <item>
    /// <term>ConversationId</term>
    /// <description>The ID of the conversation to send the message to.</description>
    /// </item>
    /// <item>
    /// <term>Content</term>
    /// <description>The text content of the message.</description>
    /// </item>
    /// <item>
    /// <term>MediaType</term>
    /// <description>The type of media (Text, Image, Video, BookingRequest, Attachment).</description>
    /// </item>
    /// <item>
    /// <term>ReplyToMessageId</term>
    /// <description>Optional - ID of the message being replied to.</description>
    /// </item>
    /// <item>
    /// <term>ReplyToMessageContent</term>
    /// <description>Optional - Content of the message being replied to (for display purposes).</description>
    /// </item>
    /// <item>
    /// <term>ReplyToMessageMediaType</term>
    /// <description>Optional - Media type of the message being replied to.</description>
    /// </item>
    /// <item>
    /// <term>CustomerPurchasedId</term>
    /// <description>Optional - ID of the customer purchase (for booking request messages).</description>
    /// </item>
    /// <item>
    /// <term>CreateBookingRequest</term>
    /// <description>Optional - Booking request details if this is a booking request message.</description>
    /// </item>
    /// </list>
    /// </param>
    /// <returns>Success confirmation.</returns>
    /// <remarks>
    /// Sends a message and notifies all conversation members via SignalR.
    /// The message status is automatically set to "Sent" for the sender and "None" for other members.
    ///
    /// For booking request messages, include the CustomerPurchasedId and CreateBookingRequest details.
    ///
    /// Sample request (text message):
    ///
    ///     POST /api/v1/messaging/messages
    ///     {
    ///         "conversationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "content": "Looking forward to our session tomorrow!",
    ///         "mediaType": "Text",
    ///         "replyToMessageId": null,
    ///         "replyToMessageContent": null,
    ///         "replyToMessageMediaType": null,
    ///         "customerPurchasedId": null,
    ///         "createBookingRequest": null
    ///     }
    ///
    /// Sample request (booking request):
    ///
    ///     POST /api/v1/messaging/messages
    ///     {
    ///         "conversationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "content": "Booking request",
    ///         "mediaType": "BookingRequest",
    ///         "customerPurchasedId": "3fa85f64-5717-4562-b3fc-2c963f66afa8",
    ///         "createBookingRequest": {
    ///             "bookingDate": "2024-02-15",
    ///             "ptFreelanceStartTime": "09:00",
    ///             "ptFreelanceEndTime": "10:00",
    ///             "bookingName": "Morning Workout Session"
    ///         }
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Message sent successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">Unauthorized - User not authenticated</response>
    /// <response code="404">User or conversation not found</response>
    [HttpPost("messages")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<EmptyResult>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageCommand command)
    {
        await _mediator.Send(command);
        return Ok(new BaseResponse<EmptyResult>(
            StatusCodes.Status200OK.ToString(),
            "Message sent successfully",
            Empty));
    }

    /// <summary>
    /// Updates the content of an existing message.
    /// </summary>
    /// <param name="messageId">The ID of the message to update.</param>
    /// <param name="command">The update details including:
    /// <list type="bullet">
    /// <item>
    /// <term>ConversationId</term>
    /// <description>The ID of the conversation containing the message.</description>
    /// </item>
    /// <item>
    /// <term>NewContent</term>
    /// <description>The new content for the message.</description>
    /// </item>
    /// </list>
    /// </param>
    /// <returns>Success confirmation.</returns>
    /// <remarks>
    /// Only the message sender can update their own messages.
    /// If the message is the last message in the conversation, the conversation's last message content is also updated.
    /// All conversation members are notified via SignalR about the message update.
    ///
    /// Sample request:
    ///
    ///     PUT /api/v1/messaging/messages/3fa85f64-5717-4562-b3fc-2c963f66afa7
    ///     {
    ///         "conversationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "newContent": "Updated: Looking forward to our session tomorrow at 9 AM!"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Message updated successfully</response>
    /// <response code="400">Invalid request - sender mismatch</response>
    /// <response code="401">Unauthorized - User not authenticated</response>
    /// <response code="404">User, message, or conversation not found</response>
    [HttpPut("messages/{messageId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<EmptyResult>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMessage([FromRoute] Guid messageId, [FromBody] UpdateMessageComand command)
    {
        command.MessageId = messageId;
        await _mediator.Send(command);
        return Ok(new BaseResponse<EmptyResult>(
            StatusCodes.Status200OK.ToString(),
            "Message updated successfully",
            Empty));
    }

    /// <summary>
    /// Soft deletes a message by marking it as deleted.
    /// </summary>
    /// <param name="messageId">The ID of the message to delete.</param>
    /// <returns>Success confirmation.</returns>
    /// <remarks>
    /// Only the message sender can delete their own messages.
    /// The message is soft-deleted (marked with DeletedAt timestamp) rather than permanently removed.
    /// If the message is the last message in the conversation, the conversation's UpdatedAt is updated.
    /// All conversation members are notified via SignalR about the message deletion.
    ///
    /// Sample request:
    ///
    ///     DELETE /api/v1/messaging/messages/3fa85f64-5717-4562-b3fc-2c963f66afa7
    ///
    /// </remarks>
    /// <response code="200">Message deleted successfully</response>
    /// <response code="400">Invalid request - sender mismatch</response>
    /// <response code="401">Unauthorized - User not authenticated</response>
    /// <response code="404">User or message not found</response>
    [HttpDelete("messages/{messageId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<EmptyResult>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMessage([FromRoute] Guid messageId)
    {
        var command = new DeleteMessageCommand { MessageId = messageId };
        await _mediator.Send(command);
        return Ok(new BaseResponse<EmptyResult>(
            StatusCodes.Status200OK.ToString(),
            "Message deleted successfully",
            Empty));
    }

    #endregion

    #region Message Interactions

    /// <summary>
    /// Marks multiple messages as read in a conversation.
    /// </summary>
    /// <param name="command">The read status update including:
    /// <list type="bullet">
    /// <item>
    /// <term>ConversationId</term>
    /// <description>The ID of the conversation containing the messages.</description>
    /// </item>
    /// <item>
    /// <term>MessageIds</term>
    /// <description>List of message IDs to mark as read.</description>
    /// </item>
    /// </list>
    /// </param>
    /// <returns>Success confirmation.</returns>
    /// <remarks>
    /// Updates the message status for the current user to "Read" with a ReadAt timestamp.
    /// Only updates messages that haven't been read yet.
    /// All conversation members are notified via SignalR about the read status update.
    ///
    /// Sample request:
    ///
    ///     POST /api/v1/messaging/messages/read
    ///     {
    ///         "conversationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "messageIds": [
    ///             "3fa85f64-5717-4562-b3fc-2c963f66afa7",
    ///             "3fa85f64-5717-4562-b3fc-2c963f66afa8",
    ///             "3fa85f64-5717-4562-b3fc-2c963f66afa9"
    ///         ]
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Messages marked as read successfully</response>
    /// <response code="401">Unauthorized - User not authenticated</response>
    /// <response code="404">User not found</response>
    [HttpPost("messages/read")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<EmptyResult>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReadMessages([FromBody] ReadMessagesCommand command)
    {
        await _mediator.Send(command);
        return Ok(new BaseResponse<EmptyResult>(
            StatusCodes.Status200OK.ToString(),
            "Messages marked as read successfully",
            Empty));
    }

    /// <summary>
    /// Adds or removes a reaction (emoji) to/from a message.
    /// </summary>
    /// <param name="command">The reaction details including:
    /// <list type="bullet">
    /// <item>
    /// <term>MessageId</term>
    /// <description>The ID of the message to react to.</description>
    /// </item>
    /// <item>
    /// <term>ConversationId</term>
    /// <description>The ID of the conversation containing the message.</description>
    /// </item>
    /// <item>
    /// <term>Reaction</term>
    /// <description>The reaction emoji (e.g., "❤️", "👍", "😂"). Null when removing reaction.</description>
    /// </item>
    /// <item>
    /// <term>RemoveReaction</term>
    /// <description>Set to true to remove the reaction, false to add it (default: false).</description>
    /// </item>
    /// </list>
    /// </param>
    /// <returns>Success confirmation.</returns>
    /// <remarks>
    /// Each user can only have one reaction per message. Adding a new reaction replaces the previous one.
    /// All conversation members are notified via SignalR when a reaction is added or removed.
    ///
    /// Sample request (add reaction):
    ///
    ///     POST /api/v1/messaging/messages/react
    ///     {
    ///         "messageId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
    ///         "conversationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "reaction": "❤️",
    ///         "removeReaction": false
    ///     }
    ///
    /// Sample request (remove reaction):
    ///
    ///     POST /api/v1/messaging/messages/react
    ///     {
    ///         "messageId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
    ///         "conversationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "reaction": null,
    ///         "removeReaction": true
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Reaction added/removed successfully</response>
    /// <response code="401">Unauthorized - User not authenticated</response>
    /// <response code="404">User or message not found</response>
    [HttpPost("messages/react")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<EmptyResult>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReactToMessage([FromBody] ReactMessageCommand command)
    {
        await _mediator.Send(command);
        return Ok(new BaseResponse<EmptyResult>(
            StatusCodes.Status200OK.ToString(),
            command.RemoveReaction ? "Reaction removed successfully" : "Reaction added successfully",
            Empty));
    }

    #endregion
}