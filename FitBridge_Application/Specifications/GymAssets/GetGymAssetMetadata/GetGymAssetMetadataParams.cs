using System;
using FitBridge_Domain.Enums.Gyms;

namespace FitBridge_Application.Specifications.GymAssets.GetGymAssetMetadata;

public class GetGymAssetMetadataParams : BaseParams
{
    public Guid? AssetMetadataId { get; set; }
    public AssetType? AssetType { get; set; }
    public EquipmentCategoryType? EquipmentCategoryType { get; set; }
}
