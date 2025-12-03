using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.GymAssets;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.GymAssets.GetGymAssets;
using FitBridge_Domain.Entities.Gyms;
using MediatR;

namespace FitBridge_Application.Features.GymAssets.GetGymAssets;

public class GetGymAssetsQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper) 
    : IRequestHandler<GetGymAssetsQuery, PagingResultDto<GymAssetResponseDto>>
{
    public async Task<PagingResultDto<GymAssetResponseDto>> Handle(
        GetGymAssetsQuery request, 
        CancellationToken cancellationToken)
    {
        var spec = new GetGymAssetsSpec(request.Params);
        
        var gymAssets = await unitOfWork.Repository<GymAsset>()
            .GetAllWithSpecificationAsync(spec);
        
        var totalCount = await unitOfWork.Repository<GymAsset>()
            .CountAsync(spec);
        
        var dtos = mapper.Map<IReadOnlyList<GymAssetResponseDto>>(gymAssets);
        
        return new PagingResultDto<GymAssetResponseDto>(totalCount, dtos);
    }
}
