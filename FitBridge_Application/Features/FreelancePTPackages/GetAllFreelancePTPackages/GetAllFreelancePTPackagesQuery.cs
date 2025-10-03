using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.FreelancePTPackages;
using FitBridge_Application.Specifications.FreelancePtPackages.GetAllFreelancePTPackages;
using MediatR;

namespace FitBridge_Application.Features.FreelancePTPackages.GetAllFreelancePTPackages
{
    public class GetAllFreelancePTPackagesQuery : IRequest<PagingResultDto<GetAllFreelancePTPackagesDto>>
    {
        public GetAllFreelancePTPackagesParam Params { get; set; }
    }
}