using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Specifications.GymAssets.GetGymAssetMetadata;

public class GetGymAssetMetadataSpec : BaseSpecification<AssetMetadata>
{
    public GetGymAssetMetadataSpec(GetGymAssetMetadataParams parameters) : base(x =>
    (parameters.AssetMetadataId == null || x.Id == parameters.AssetMetadataId)
    && (parameters.AssetType == null || x.AssetType == parameters.AssetType)
    && (parameters.EquipmentCategoryType == null || x.EquipmentCategoryType == parameters.EquipmentCategoryType)
    && (parameters.SearchTerm == null || x.Name.ToLower().Contains(parameters.SearchTerm.ToLower()))
    )
    {
        if(parameters.SortOrder == "asc") {
            AddOrderBy(x => x.Name);
        } else {
            AddOrderByDesc(x => x.Name);
        }
        
        if (parameters.DoApplyPaging)
        {
            AddPaging(parameters.Size * (parameters.Page - 1), parameters.Size);
        }
        else
        {
            parameters.Size = -1;
            parameters.Page = -1;
        }
    }
}
