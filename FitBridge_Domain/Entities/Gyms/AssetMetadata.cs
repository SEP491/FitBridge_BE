using System;
using FitBridge_Domain.Enums.Gyms;
using FitBridge_Domain.Enums.SessionActivities;

namespace FitBridge_Domain.Entities.Gyms;

public class AssetMetadata : BaseEntity
{
    public string Name { get; set; }
    public AssetType AssetType { get; set; }
    public EquipmentCategoryType? EquipmentCategoryType { get; set; }
    public string Description { get; set; }
    public List<string>? TargetMuscularGroups { get; set; } = new List<string>();
    public ICollection<GymAsset> GymAssets { get; set; } = new List<GymAsset>();
}
