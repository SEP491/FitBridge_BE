using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.GymAssets.UpdateGymAsset;

public class UpdateGymAssetCommand : IRequest<bool>
{
    public Guid GymAssetId { get; set; }
        
    public int? Quantity { get; set; }
    
    public List<string>? ImagesToRemove { get; set; }
    
    public List<IFormFile>? ImagesToAdd { get; set; }
}
