using AutoMapper;
using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Dtos.Templates;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services.Notifications;
using FitBridge_Application.Specifications.Templates;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Domain.Enums.Templates;
using FitBridge_Domain.Exceptions;
using FitBridge_Infrastructure.Services.Templating;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace FitBridge_Infrastructure.Services.Notifications
{
    internal class NotificationsService(
        ChannelWriter<NotificationMessage> channelWriter,
        IUnitOfWork unitOfWork,
        ILogger logger,
        IMapper mapper,
        TemplatingService templatingService) : INotificationService
    {
        private readonly ConcurrentDictionary<EnumContentType, (TemplateDto?, TemplateDto?)> templateDtos = [];

        public async Task NotifyUsers(NotificationMessage notificationMessage)
        {
            var isExists = templateDtos.TryGetValue(notificationMessage.NotificationTypes, out var templates);
            if (!isExists)
            {
                var inAppTemplate = await GetInAppNotificationTemplate(notificationMessage.NotificationTypes, notificationMessage.TemplateModel);
                var pushTemplate = await GetPushNotificationTemplate(notificationMessage.NotificationTypes, notificationMessage.TemplateModel);
                templates = (inAppTemplate, pushTemplate);
                templateDtos.TryAdd(notificationMessage.NotificationTypes, templates);
            }

            notificationMessage.InAppNotificationTemplate = templates.Item1;
            notificationMessage.PushNotificationTemplate = templates.Item2;

            await StoreNotification(notificationMessage, notificationMessage.InAppNotificationTemplate.Id);
            await channelWriter.WriteAsync(notificationMessage);
        }

        private async Task StoreNotification(NotificationMessage notificationMessage, Guid templateId)
        {
            foreach (var userId in notificationMessage.UserIds)
            {
                var newNotification = new Notification
                {
                    Id = Guid.NewGuid(),
                    AdditionalPayload = notificationMessage.NotificationPayload,
                    Body = notificationMessage.InAppNotificationTemplate?.TemplateBody ?? "Empty body",
                    Title = notificationMessage.InAppNotificationTemplate?.TemplateTitle ?? "Empty title",
                    TemplateId = templateId,
                    UserId = userId
                };
                unitOfWork.Repository<Notification>().Insert(newNotification);
            }
            await unitOfWork.CommitAsync();
        }

        private async Task<TemplateDto?> GetInAppNotificationTemplate(EnumContentType contentType, IBaseTemplateModel model)
        {
            var spec = new GetByTemplateTypeSpecification(contentType, TemplateCategory.InAppNotification);
            var template = await unitOfWork.Repository<Template>().GetBySpecificationProjectedAsync<TemplateDto>(spec, mapper.ConfigurationProvider);
            if (template == null)
            {
                logger.LogWarning("In-app template is not available for {ContentType}", contentType.ToString());
                return null;
            }

            template.TemplateBody = await templatingService.ParseTemplateAsync(template.TemplateBody, model);
            return template;
        }

        private async Task<TemplateDto?> GetPushNotificationTemplate(EnumContentType contentType, IBaseTemplateModel model)
        {
            var spec = new GetByTemplateTypeSpecification(contentType, TemplateCategory.PushNotification);
            var template = await unitOfWork.Repository<Template>().GetBySpecificationProjectedAsync<TemplateDto>(spec, mapper.ConfigurationProvider)
                ?? throw new NotFoundException(nameof(Template));

            if (template == null)
            {
                logger.LogWarning("Push notification template is not available for {ContentType}", contentType.ToString());
                return null;
            }

            template.TemplateBody = await templatingService.ParseTemplateAsync(template.TemplateBody, model);
            return template;
        }
    }
}