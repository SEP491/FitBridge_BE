using AutoMapper;
using FitBridge_Application.Dtos.GymAssets;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.MappingProfiles;

public class GymAssetMappingProfile : Profile
{
    public GymAssetMappingProfile()
    {
        CreateMap<GymAsset, GymAssetResponseDto>()
            .ForMember(dest => dest.GymOwnerName, opt => opt.MapFrom(src => src.GymOwner.FullName))
            .ForMember(dest => dest.AssetName, opt => opt.MapFrom(src => src.AssetMetadata.Name))
            .ForMember(dest => dest.AssetType, opt => opt.MapFrom(src => src.AssetMetadata.AssetType))
            .ForMember(dest => dest.EquipmentCategory, opt => opt.MapFrom(src => src.AssetMetadata.EquipmentCategoryType))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.AssetMetadata.Description))
            .ForMember(dest => dest.TargetMuscularGroups, opt => opt.MapFrom(src => src.AssetMetadata.TargetMuscularGroups));
        CreateMap<AssetMetadata, AssetMetadataDto>();
        CreateProjection<AssetMetadata, AssetMetadataBrief>();
    }
}
