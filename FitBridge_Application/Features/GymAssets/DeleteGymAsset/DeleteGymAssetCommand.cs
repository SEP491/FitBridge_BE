using System.Text.Json.Serialization;
using MediatR;

namespace FitBridge_Application.Features.GymAssets.DeleteGymAsset;

public class DeleteGymAssetCommand : IRequest<bool>
{
    public Guid GymAssetId { get; set; }    
}
