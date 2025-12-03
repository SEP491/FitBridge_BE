using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.GymAssets;
using FitBridge_Application.Specifications.GymAssets.GetGymAssets;
using MediatR;

namespace FitBridge_Application.Features.GymAssets.GetGymAssets;

public class GetGymAssetsQuery : IRequest<PagingResultDto<GymAssetResponseDto>>
{
    public GetGymAssetsParams Params { get; set; }
}
