using FitBridge_Application.Specifications;
using FitBridge_Domain.Enums.Gyms;

namespace FitBridge_Application.Specifications.GymAssets.GetGymAssets;

public class GetGymAssetsParams : BaseParams
{
    public Guid? GymOwnerId { get; set; }
    public Guid? GymAssetId { get; set; }
    public Guid? AssetMetadataId { get; set; }
    public AssetType? AssetType { get; set; }
}
