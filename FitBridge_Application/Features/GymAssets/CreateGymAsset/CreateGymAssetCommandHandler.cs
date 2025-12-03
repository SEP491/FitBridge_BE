using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.GymAssets.CreateGymAsset;

public class CreateGymAssetCommandHandler(
    IUnitOfWork unitOfWork,
    IUploadService uploadService,
    IApplicationUserService applicationUserService) 
    : IRequestHandler<CreateGymAssetCommand, Guid>
{
    public async Task<Guid> Handle(CreateGymAssetCommand request, CancellationToken cancellationToken)
    {
        // Validate AssetMetadata exists
        var assetMetadata = await unitOfWork.Repository<AssetMetadata>().GetByIdAsync(request.AssetMetadataId);
        if (assetMetadata == null)
        {
            throw new NotFoundException(nameof(AssetMetadata), request.AssetMetadataId);
        }

        // Validate GymOwner exists
        var gymOwner = await applicationUserService.GetByIdAsync(request.GymOwnerId, includes: new List<string> { "GymAssets" });
        if (gymOwner == null)
        {
            throw new NotFoundException("Gym owner not found");
        }
        if(gymOwner.GymAssets.Any(x => x.AssetMetadataId == request.AssetMetadataId))
        {
            throw new DataValidationFailedException("Gym owner already has this asset");
        }

        // Validate quantity
        if (request.Quantity <= 0)
        {
            throw new DataValidationFailedException("Quantity must be greater than 0");
        }

        // Create GymAsset
        var gymAsset = new GymAsset
        {
            GymOwnerId = request.GymOwnerId,
            AssetMetadataId = request.AssetMetadataId,
            Quantity = request.Quantity,
            ImageUrls = new List<string>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Handle image uploads
        if (request.ImagesToAdd != null && request.ImagesToAdd.Any())
        {
            foreach (var file in request.ImagesToAdd)
            {
                var uploadedUrl = await uploadService.UploadFileAsync(file);
                gymAsset.ImageUrls.Add(uploadedUrl);
            }
        }

        unitOfWork.Repository<GymAsset>().Insert(gymAsset);
        await unitOfWork.CommitAsync();

        return gymAsset.Id;
    }
}
