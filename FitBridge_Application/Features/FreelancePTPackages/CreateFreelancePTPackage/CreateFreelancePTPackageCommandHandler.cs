using AutoMapper;
using FitBridge_Application.Dtos.FreelancePTPackages;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Domain.Entities.Gyms;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.FreelancePTPackages.CreateFreelancePTPackage
{
    internal class CreateFreelancePTPackageCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserUtil userUtil,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<CreateFreelancePTPackageCommand, CreateFreelancePTPackageDto>
    {
        public async Task<CreateFreelancePTPackageDto> Handle(CreateFreelancePTPackageCommand request, CancellationToken cancellationToken)
        {
            var accountId = userUtil.GetAccountId(httpContextAccessor.HttpContext);
            var newPackage = new FreelancePTPackage
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Price = request.Price,
                DurationInDays = request.DurationInDays,
                SessionDurationInMinutes = request.SessionDurationInMinutes,
                NumOfSessions = request.NumOfSessions,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                PtId = accountId!.Value,
                IsDisplayed = request.IsDisplayed
            };

            unitOfWork.Repository<FreelancePTPackage>().Insert(newPackage);

            await unitOfWork.CommitAsync();

            var dto = mapper.Map<CreateFreelancePTPackageDto>(newPackage);
            return dto;
        }
    }
}