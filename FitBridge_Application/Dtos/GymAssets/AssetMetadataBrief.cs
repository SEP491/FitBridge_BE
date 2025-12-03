using System;
using FitBridge_Domain.Enums.Gyms;

namespace FitBridge_Application.Dtos.GymAssets;

public class AssetMetadataBrief
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public AssetType AssetType { get; set; }
}
