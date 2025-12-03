using System.Text.Json.Serialization;
using FitBridge_Application.Dtos.GymAssets;
using FitBridge_Domain.Enums.Gyms;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.GymAssets.CreateGymAsset;

public class CreateGymAssetCommand : IRequest<Guid>
{
    public Guid GymOwnerId { get; set; }
    public List<IFormFile>? ImagesToAdd { get; set; }
    public Guid AssetMetadataId { get; set; }
    public int Quantity { get; set; }
}
