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
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => new DateTimeOffset(src.CreatedAt).ToUnixTimeMilliseconds()))
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.ReadAt.HasValue));
        }
    }
}