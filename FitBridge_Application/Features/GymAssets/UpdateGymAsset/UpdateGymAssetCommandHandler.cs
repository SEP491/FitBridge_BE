using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Specifications.GymAssets.GetGymAssetById;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.GymAssets.UpdateGymAsset;

public class UpdateGymAssetCommandHandler(
    IUnitOfWork unitOfWork,
    IUploadService uploadService,
    IApplicationUserService applicationUserService) 
    : IRequestHandler<UpdateGymAssetCommand, bool>
{
    public async Task<bool> Handle(UpdateGymAssetCommand request, CancellationToken cancellationToken)
    {
        var spec = new GetGymAssetByIdSpec(request.GymAssetId);
        var gymAsset = await unitOfWork.Repository<GymAsset>()
            .GetBySpecificationAsync(spec, asNoTracking: false);
        
        if (gymAsset == null)
        {
            throw new NotFoundException(nameof(GymAsset), request.GymAssetId);
        }
        if (request.Quantity.HasValue)
        {
            if (request.Quantity.Value <= 0)
            {
                throw new DataValidationFailedException("Quantity must be greater than 0");
            }
            gymAsset.Quantity = request.Quantity.Value;
        }

        await HandleImagesUpdate(gymAsset, request);

        gymAsset.UpdatedAt = DateTime.UtcNow;

        unitOfWork.Repository<GymAsset>().Update(gymAsset);
        await unitOfWork.CommitAsync();

        return true;
    }

    private async Task HandleImagesUpdate(GymAsset gymAsset, UpdateGymAssetCommand request)
    {
        gymAsset.ImageUrls ??= new List<string>();

        if (request.ImagesToRemove != null && request.ImagesToRemove.Any())
        {
            if (gymAsset.ImageUrls.Count == 0)
            {
                throw new BusinessException("No images to remove");
            }

            foreach (var imageUrl in request.ImagesToRemove)
            {
                if (gymAsset.ImageUrls.Contains(imageUrl))
                {
                    await uploadService.DeleteFileAsync(imageUrl);
                    
                    gymAsset.ImageUrls.Remove(imageUrl);
                }
            }
        }

        if (request.ImagesToAdd != null && request.ImagesToAdd.Any())
        {
            foreach (var file in request.ImagesToAdd)
            {
                var uploadedUrl = await uploadService.UploadFileAsync(file);
                
                gymAsset.ImageUrls.Add(uploadedUrl);
            }
        }
    }
}
