using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.FreelancePTPackages;
using FitBridge_Application.Features.FreelancePTPackages.CreateFreelancePTPackage;
using FitBridge_Application.Features.FreelancePTPackages.UpdateFreelancePTPackage;
using FitBridge_Application.Features.FreelancePTPackages.DeleteFreelancePTPackage;
using FitBridge_Application.Features.FreelancePTPackages.GetFreelancePTPackageById;
using FitBridge_Application.Features.FreelancePTPackages.GetAllFreelancePTPackages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitBridge_Application.Specifications.FreelancePtPackages.GetAllFreelancePTPackages;

namespace FitBridge_API.Controllers
{
    [Authorize]
    public class FreelancePTPackagesController(IMediator mediator) : _BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetFreelancePTPackages([FromQuery] GetAllFreelancePTPackagesParam parameters)
        {
            var result = await mediator.Send(new GetAllFreelancePTPackagesQuery { Params = parameters });
            var pagination = ResultWithPagination(result.Items, result.Total, parameters.Page, parameters.Size);
            return Ok(
                new BaseResponse<Pagination<GetAllFreelancePTPackagesDto>>(
                    StatusCodes.Status200OK.ToString(),
                    "Freelance PT Packages retrieved successfully",
                    pagination));
        }

        [HttpPost]
        public async Task<IActionResult> CreateFreelancePTPackage([FromBody] CreateFreelancePTPackageCommand command)
        {
            var packageDto = await mediator.Send(command);
            return Created(
                nameof(CreateFreelancePTPackage),
                new BaseResponse<CreateFreelancePTPackageDto>(
                    StatusCodes.Status201Created.ToString(),
                    "Freelance PT Package created successfully",
                    packageDto));
        }

        [HttpPut("{packageId}")]
        public async Task<IActionResult> UpdateFreelancePTPackage([FromRoute] Guid packageId, [FromBody] UpdateFreelancePTPackageCommand updateCommand)
        {
            updateCommand.PackageId = packageId;
            await mediator.Send(updateCommand);
            return Ok(
                new BaseResponse<EmptyResult>(
                    StatusCodes.Status200OK.ToString(),
                    "Freelance PT Package updated successfully",
                    Empty));
        }

        [HttpGet("{packageId}")]
        public async Task<IActionResult> GetFreelancePTPackageById([FromRoute] Guid packageId)
        {
            var result = await mediator.Send(new GetFreelancePTPackageByIdQuery { PackageId = packageId });
            return Ok(
                new BaseResponse<GetFreelancePTPackageByIdDto>(
                    StatusCodes.Status200OK.ToString(),
                    "Freelance PT Package retrieved successfully",
                    result));
        }

        [HttpDelete("{packageId}")]
        public async Task<IActionResult> DeleteFreelancePTPackage([FromRoute] Guid packageId)
        {
            await mediator.Send(new DeleteFreelancePTPackageCommand { PackageId = packageId });
            return Ok(
                new BaseResponse<EmptyResult>(
                    StatusCodes.Status200OK.ToString(),
                    "Freelance PT Package deleted successfully",
                    Empty));
        }
    }
}