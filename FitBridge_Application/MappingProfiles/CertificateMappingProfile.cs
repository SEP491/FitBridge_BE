using System;
using AutoMapper;
using FitBridge_Application.Features.Certificates.AddCertificateRequest;
using FitBridge_Application.Dtos.Certificates;
using FitBridge_Domain.Entities.Certificates;

namespace FitBridge_Application.MappingProfiles;

public class CertificateMappingProfile : Profile
{

    public CertificateMappingProfile()
    {
        CreateMap<AddCertificateRequestCommand, PtCertificates>()
        .ForMember(dest => dest.PtId, opt => opt.MapFrom(src => src.PtId))
        .ForMember(dest => dest.CertificateMetadataId, opt => opt.MapFrom(src => src.CertificateMetadataId))
        .ForMember(dest => dest.CertUrl, opt => opt.MapFrom(src => src.CertUrl))
        .ForMember(dest => dest.ProvidedDate, opt => opt.MapFrom(src => src.ProvidedDate))
        .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate));

        CreateMap<PtCertificates, CertificatesDto>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.PtId, opt => opt.MapFrom(src => src.PtId))
        .ForMember(dest => dest.CertificateMetadataId, opt => opt.MapFrom(src => src.CertificateMetadataId))
        .ForMember(dest => dest.CertUrl, opt => opt.MapFrom(src => src.CertUrl))
        .ForMember(dest => dest.ProvidedDate, opt => opt.MapFrom(src => src.ProvidedDate))
        .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate))
        .ForMember(dest => dest.CertificateStatus, opt => opt.MapFrom(src => src.CertificateStatus))
        .ForMember(dest => dest.CertificateMetadata, opt => opt.MapFrom(src => src.CertificateMetadata))
        .ForMember(dest => dest.PtName, opt => opt.MapFrom(src => src.Pt.FullName))
        .ForMember(dest => dest.PtImageUrl, opt => opt.MapFrom(src => src.Pt.AvatarUrl));

        
        CreateMap<CertificateMetadata, CertificateMetadataDto>();
    }
}
