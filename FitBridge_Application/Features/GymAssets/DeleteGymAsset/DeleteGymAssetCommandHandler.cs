using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Specifications.GymAssets.GetGymAssetById;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.GymAssets.DeleteGymAsset;

public class DeleteGymAssetCommandHandler(
    IUnitOfWork unitOfWork,
    IUploadService uploadService,
    IApplicationUserService applicationUserService) 
    : IRequestHandler<DeleteGymAssetCommand, bool>
{
    public async Task<bool> Handle(DeleteGymAssetCommand request, CancellationToken cancellationToken)
    {
        var spec = new GetGymAssetByIdSpec(request.GymAssetId);
        var gymAsset = await unitOfWork.Repository<GymAsset>()
            .GetBySpecificationAsync(spec, asNoTracking: false);
        
        if (gymAsset == null)
        {
            throw new NotFoundException(nameof(GymAsset), request.GymAssetId);
        }

        if (gymAsset.ImageUrls != null && gymAsset.ImageUrls.Any())
        {
            foreach (var imageUrl in gymAsset.ImageUrls)
            {
                try
                {
                    await uploadService.DeleteFileAsync(imageUrl);
                }
                catch (Exception)
                {
                }
            }
        }

        unitOfWork.Repository<GymAsset>().Delete(gymAsset);
        await unitOfWork.CommitAsync();

        return true;
    }
}
