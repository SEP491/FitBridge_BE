using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.GymAssets;
using FitBridge_Application.Specifications.GymAssets.GetGymAssetMetadata;
using MediatR;

namespace FitBridge_Application.Features.GymAssets.GetGymAssetMetadata;

public class GetGymAssetMetadataQuery : IRequest<PagingResultDto<AssetMetadataBrief>>
{
    public GetGymAssetMetadataParams Params { get; set; }   
}
