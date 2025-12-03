using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Enums.Gyms;

namespace FitBridge_Application.Specifications.GymAssets.GetGymAssets;

public class GetGymAssetsSpec : BaseSpecification<GymAsset>
{
    public GetGymAssetsSpec(GetGymAssetsParams parameters) 
        : base(x =>
            (parameters.GymOwnerId == null || x.GymOwnerId == parameters.GymOwnerId) &&
            (parameters.AssetMetadataId == null || x.AssetMetadataId == parameters.AssetMetadataId) &&
            (parameters.AssetType == null || x.AssetMetadata.AssetType == parameters.AssetType) &&
            (parameters.GymAssetId == null || x.Id == parameters.GymAssetId)
        )
    {
        AddInclude(x => x.GymOwner);
        AddInclude(x => x.AssetMetadata);
        
        if (parameters.SortOrder?.ToLower() == "asc")
        {
            AddOrderBy(x => x.CreatedAt);
        }
        else
        {
            AddOrderByDesc(x => x.CreatedAt);
        }
        
        if (parameters.DoApplyPaging)
        {
            AddPaging((parameters.Page - 1) * parameters.Size, parameters.Size);
        }
    }
}
