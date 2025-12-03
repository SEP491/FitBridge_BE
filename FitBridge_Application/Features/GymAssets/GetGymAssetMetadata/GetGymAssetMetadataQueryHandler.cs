using System;
using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.GymAssets;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.GymAssets.GetGymAssetMetadata;
using MediatR;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Features.GymAssets.GetGymAssetMetadata;

public class GetGymAssetMetadataQueryHandler(
    IUnitOfWork unitOfWork, 
    IMapper mapper) 
    : IRequestHandler<GetGymAssetMetadataQuery, PagingResultDto<AssetMetadataBrief>>
{
    public async Task<PagingResultDto<AssetMetadataBrief>> Handle(GetGymAssetMetadataQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetGymAssetMetadataSpec(request.Params);
        var assetMetadata = await unitOfWork.Repository<AssetMetadata>().GetAllWithSpecificationProjectedAsync<AssetMetadataBrief>(spec, mapper.ConfigurationProvider);
        var totalCount = await unitOfWork.Repository<AssetMetadata>().CountAsync(spec);
        return new PagingResultDto<AssetMetadataBrief>(totalCount, assetMetadata);
    }
}
