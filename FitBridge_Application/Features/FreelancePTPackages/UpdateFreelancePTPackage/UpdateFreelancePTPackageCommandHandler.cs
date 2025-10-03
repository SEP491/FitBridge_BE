using AutoMapper;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.FreelancePtPackages.GetFreelancePtPackageById;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.FreelancePTPackages.UpdateFreelancePTPackage
{
    internal class UpdateFreelancePTPackageCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<UpdateFreelancePTPackageCommand>
    {
        public async Task Handle(UpdateFreelancePTPackageCommand request, CancellationToken cancellationToken)
        {
            var spec = new GetFreelancePtPackageByIdSpec(request.PackageId);
            var existingPackage = await unitOfWork.Repository<FreelancePTPackage>().GetBySpecificationAsync(spec, asNoTracking: false)
                ?? throw new NotFoundException(nameof(FreelancePTPackage));

            if (!string.IsNullOrEmpty(request.Name))
            {
                existingPackage.Name = request.Name;
            }

            if (request.Price is decimal price && price > 0)
            {
                existingPackage.Price = price;
            }
            if (request.DurationInDays is int duration && duration > 0)
            {
                existingPackage.DurationInDays = duration;
            }
            if (request.SessionDurationInMinutes is int sessionDuration && sessionDuration > 0)
            {
                existingPackage.SessionDurationInMinutes = sessionDuration;
            }
            if (request.NumOfSessions is int sessions && sessions > 0)
            {
                existingPackage.NumOfSessions = sessions;
            }
            if (!string.IsNullOrEmpty(request.Description))
            {
                existingPackage.Description = request.Description;
            }
            if (!string.IsNullOrEmpty(request.ImageUrl))
            {
                existingPackage.ImageUrl = request.ImageUrl;
            }

            unitOfWork.Repository<FreelancePTPackage>().Update(existingPackage);

            await unitOfWork.CommitAsync();
        }
    }
}