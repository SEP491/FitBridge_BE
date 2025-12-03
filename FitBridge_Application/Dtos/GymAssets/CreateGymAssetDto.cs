using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Dtos.GymAssets;

public class CreateGymAssetDto
{
    public Guid AssetMetadataId { get; set; }
    public int Quantity { get; set; }
    public List<IFormFile>? Images { get; set; }
}
