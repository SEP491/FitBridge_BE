using System;
using FitBridge_Domain.Enums.Gyms;

namespace FitBridge_Application.Dtos.GymAssets;

public class AssetMetadataDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public AssetType AssetType { get; set; }
    public EquipmentCategoryType EquipmentCategoryType { get; set; }
    public string Description { get; set; }
    public List<string> TargetMuscularGroups { get; set; } = new List<string>();
}
