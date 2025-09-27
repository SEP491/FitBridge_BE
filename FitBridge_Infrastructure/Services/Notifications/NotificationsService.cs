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
using System.Collections.Concurrent;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace FitBridge_Infrastructure.Services.Notifications
{
    internal class NotificationsService(
        ChannelWriter<NotificationMessage> channelWriter,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        TemplatingService templatingService) : INotificationService
    {
        private readonly ConcurrentDictionary<EnumContentType, (TemplateDto, TemplateDto)> templateDtos = [];

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

            await channelWriter.WriteAsync(notificationMessage);
        }

        private async Task<TemplateDto> GetInAppNotificationTemplate(EnumContentType contentType, dynamic model)
        {
            var spec = new GetByTemplateTypeSpecification(contentType, TemplateCategory.InAppNotification);
            var template = await unitOfWork.Repository<Template>().GetBySpecificationProjectedAsync<TemplateDto>(spec, mapper.ConfigurationProvider)
                ?? throw new NotFoundException(nameof(Template));

            var templateModel = TemplatingService.GetTemplateModel(contentType, model);
            template.TemplateBody = await templatingService.ParseTemplateAsync(template.TemplateBody, templateModel);
            return template;
        }

        private async Task<TemplateDto> GetPushNotificationTemplate(EnumContentType contentType, dynamic model)
        {
            var spec = new GetByTemplateTypeSpecification(contentType, TemplateCategory.PushNotification);
            var template = await unitOfWork.Repository<Template>().GetBySpecificationProjectedAsync<TemplateDto>(spec, mapper.ConfigurationProvider)
                ?? throw new NotFoundException(nameof(Template));

            var templateModel = TemplatingService.GetTemplateModel(contentType, model);
            template.TemplateBody = await templatingService.ParseTemplateAsync(template.TemplateBody, templateModel);
            return template;
        }
    }
}