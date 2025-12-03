using FitBridge_Domain.Enums.Gyms;

namespace FitBridge_Application.Dtos.GymAssets;

public class GymAssetResponseDto
{
    public Guid Id { get; set; }
    public Guid GymOwnerId { get; set; }
    public string GymOwnerName { get; set; }
    public Guid AssetMetadataId { get; set; }
    public string AssetName { get; set; }
    public AssetType AssetType { get; set; }
    public EquipmentCategoryType EquipmentCategory { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public List<string> ImageUrls { get; set; } = new List<string>();
    public List<string> TargetMuscularGroups { get; set; } = new List<string>();
    public DateTime CreatedAt { get; set; }
}
