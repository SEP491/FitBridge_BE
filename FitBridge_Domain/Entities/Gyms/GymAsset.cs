using System;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Domain.Entities.Gyms;

public class GymAsset : BaseEntity
{
    public Guid GymOwnerId { get; set; }
    public Guid AssetMetadataId { get; set; }
    public int Quantity { get; set; }
    public List<string> ImageUrls { get; set; } = new List<string>();
    public ApplicationUser GymOwner { get; set; }
    public AssetMetadata AssetMetadata { get; set; }

}
