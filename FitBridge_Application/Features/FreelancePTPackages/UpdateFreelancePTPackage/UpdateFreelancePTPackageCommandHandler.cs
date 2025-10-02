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

            if (request.Price > 0)
            {
                existingPackage.Price = request.Price;
            }
            if (request.DurationInDays > 0)
            {
                existingPackage.DurationInDays = request.DurationInDays;
            }
            if (request.SessionDurationInMinutes > 0)
            {
                existingPackage.SessionDurationInMinutes = request.SessionDurationInMinutes;
            }
            if (request.NumOfSessions > 0)
            {
                existingPackage.NumOfSessions = request.NumOfSessions;
            }
            if (request.Description is not null)
            {
                existingPackage.Description = request.Description;
            }
            if (request.ImageUrl is not null)
            {
                existingPackage.ImageUrl = request.ImageUrl;
            }

            unitOfWork.Repository<FreelancePTPackage>().Update(existingPackage);

            await unitOfWork.CommitAsync();
        }
    }
}