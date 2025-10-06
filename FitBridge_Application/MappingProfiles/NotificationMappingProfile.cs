using AutoMapper;
using FitBridge_Application.Dtos.Notifications;
using FitBridge_Domain.Entities.MessageAndReview;
using Newtonsoft.Json;

namespace FitBridge_Application.MappingProfiles
{
    public class NotificationMappingProfile : Profile
    {
        public NotificationMappingProfile()
        {
            CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Template.TemplateTile))
                .ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.Template.TemplateBody))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.ReadAt.HasValue))
                .ForMember(dest => dest.AdditionalPayload, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.AdditionalPayload) ? null : JsonConvert.DeserializeObject<Dictionary<string, string>>(src.AdditionalPayload)));
        }
    }
}