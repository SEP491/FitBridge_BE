using FitBridge_Application.Dtos.Messaging;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services.Messaging;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Messaging.GetConversations;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Messaging.CreateConversation
{
    internal class CreateConversationCommandHandler(
        IUserUtil userUtil,
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork,
        IMessagingHubService messagingHubService) : IRequestHandler<CreateConversationCommand, CreateConversationResponse>
    {
        public async Task<CreateConversationResponse> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
        {
            var senderId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                ?? throw new NotFoundException(nameof(ApplicationUser));
            var senderAvatar = userUtil.GetAvatarUrl(httpContextAccessor.HttpContext)
                ?? throw new NotFoundException("User avatar");
            var now = DateTime.UtcNow;
            if (request.Members.Count < 2)
            {
                throw new DataValidationFailedException("Need at least 2 members");
            }
            if (request.Members.Count > 2 && !request.IsGroup)
            {
                throw new DataValidationFailedException("A conversation with more than 2 members must be a group.");
            }

            var memberIds = request.Members.Select(m => m.MemberId).ToList();
            var spec = new GetConversationSpec(request.IsGroup, memberIds);
            var existingConversation = await unitOfWork.Repository<Conversation>()
                .GetBySpecificationAsync(spec);

            if (existingConversation != null)
            {
                return new CreateConversationResponse
                {
                    ConversationId = existingConversation.Id,
                };
            }

            var newConversation = new Conversation
            {
                Id = Guid.NewGuid(),
                IsGroup = request.IsGroup,
                CreatedAt = now,
                UpdatedAt = now,
                LastMessageMediaType = MediaType.Text,
                LastMessageContent = request.NewMessageContent,
                LastMessageType = MessageType.User,
            };
            unitOfWork.Repository<Conversation>().Insert(newConversation);

            var newMessageId = Guid.NewGuid();
            var groupName = CreateGroupName(request.Members);
            var groupImage = request.GroupImage;

            // Create conversation members
            var conversationMembers = new List<ConversationMember>();
            for (int i = 0; i < request.Members.Count; ++i)
            {
                var member = request.Members[i];
                var conversationMember = new ConversationMember
                {
                    Id = Guid.NewGuid(), // Explicitly set the Id
                    ConversationId = newConversation.Id,
                    UserId = member.MemberId,
                    CustomTitle = !request.IsGroup ? request.Members[1 - i].MemberName : groupName, // get the other member: 1 - 0 = 1, 1- 1 = 0
                    ConversationImage = !request.IsGroup ? request.Members[1 - i].MemberAvatarUrl : groupImage,
                    CreatedAt = now,
                    UpdatedAt = now,
                };

                unitOfWork.Repository<ConversationMember>().Insert(conversationMember);
                conversationMembers.Add(conversationMember);
            }

            // Commit to save Conversation and ConversationMembers first
            await unitOfWork.CommitAsync();

            // Now create the message with the correct SenderId (ConversationMember.Id)
            var senderMember = conversationMembers.First(cm => cm.UserId == senderId);

            var newMessage = new Message
            {
                Id = newMessageId,
                Content = request.NewMessageContent,
                ConversationId = newConversation.Id,
                MediaType = MediaType.Text,
                ReplyToMessageId = null,
                CreatedAt = now,
                SenderId = senderMember.Id // Use the ConversationMember.Id
            };
            unitOfWork.Repository<Message>().Insert(newMessage);

            newConversation.LastMessageSenderId = senderMember.Id;
            newConversation.LastMessageSenderName = request.Members.First(m => m.MemberId == senderId).MemberName;
            newConversation.LastMessageId = newMessageId;
            unitOfWork.Repository<Conversation>().Update(newConversation);

            var newMessageStatus = new MessageStatus
            {
                Id = Guid.NewGuid(),
                MessageId = newMessage.Id,
                UserId = senderMember.Id,
                CurrentStatus = CurrentMessageStatus.Sent,
                ReadAt = now,
                SentAt = now,
                DeliveredAt = null
            };
            unitOfWork.Repository<MessageStatus>().Insert(newMessageStatus);

            // Commit the message and status
            await unitOfWork.CommitAsync();

            var newConvoDto = new NewConversationDto
            {
                ConversationImg = groupImage ?? senderAvatar,
                IsGroup = request.IsGroup
            };

            var requestMessage = new MessageReceivedDto
            {
                Id = newMessageId,
                ConversationId = newConversation.Id,
                MessageType = MessageType.User.ToString(),
                Content = request.NewMessageContent,
                CreatedAt = now,
                MediaType = MediaType.Text.ToString(),
                Metadata = null,
                ReplyToMessageId = null,
                SenderAvatarUrl = senderAvatar,
                SenderId = senderId,
                SenderName = request.Members.First(m => m.MemberId.ToString()
                                                    == senderId.ToString()).MemberName,
                NewConversation = newConvoDto,
            };
            await messagingHubService.NotifyUsers(requestMessage, request.Members.Select(x => x.MemberId.ToString())
                                                                                           .ToList());

            return new CreateConversationResponse
            {
                ConversationId = newConversation.Id
            };
        }

        private string CreateGroupName(List<CreateConversationMemberCommand> createConversationCommands)
        {
            var names = createConversationCommands.Select(c => c.MemberName).ToList();
            return string.Join(", ", names);
        }
    }
}