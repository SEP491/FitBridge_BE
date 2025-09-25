using AutoMapper;
using FitBridge_Application.Dtos.Templates;
using FitBridge_Domain.Entities.MessageAndReview;

namespace FitBridge_Application.MappingProfiles
{
    public class TemplateMappingProfile : Profile
    {
        public TemplateMappingProfile()
        {
            CreateMap<Template, TemplateDto>();
        }
    }
}