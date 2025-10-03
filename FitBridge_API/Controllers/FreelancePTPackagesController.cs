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
    /// <summary>
    /// Controller for managing Freelance PT Packages, including creation, retrieval, update, and deletion.
    /// </summary>
    [Authorize]
    public class FreelancePTPackagesController(IMediator mediator) : _BaseApiController
    {
        /// <summary>
        /// Retrieves a paginated list of Freelance PT Packages.
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination, including page number, size, and optional search criteria.</param>
        /// <returns>A paginated list of Freelance PT Packages.</returns>
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

        /// <summary>
        /// Creates a new Freelance PT Package with the specified details.
        /// </summary>
        /// <param name="command">The details of the Freelance PT Package to create, including the following fields:
        /// <list type="bullet">
        /// <item>
        /// <term>PackageName</term>
        /// <description>The name of the package.</description>
        /// </item>
        /// <item>
        /// <term>Price</term>
        /// <description>The price of the package.</description>
        /// </item>
        /// <item>
        /// <term>Duration</term>
        /// <description>The duration of the package in days.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>The created Freelance PT Package details.</returns>
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

        /// <summary>
        /// Updates an existing Freelance PT Package with the specified ID.
        /// </summary>
        /// <param name="packageId">The unique identifier of the Freelance PT Package to update.</param>
        /// <param name="updateCommand">The updated details of the Freelance PT Package, including the following fields:
        /// <list type="bullet">
        /// <item>
        /// <term>PackageName</term>
        /// <description>The updated name of the package (optional).</description>
        /// </item>
        /// <item>
        /// <term>Price</term>
        /// <description>The updated price of the package (optional).</description>
        /// </item>
        /// <item>
        /// <term>Duration</term>
        /// <description>The updated duration of the package in days (optional).</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>A success response if the update is successful.</returns>
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

        /// <summary>
        /// Retrieves a specific Freelance PT Package by its ID.
        /// </summary>
        /// <param name="packageId">The unique identifier of the Freelance PT Package to retrieve.</param>
        /// <returns>The details of the specified Freelance PT Package.</returns>
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

        /// <summary>
        /// Deletes a Freelance PT Package with the specified ID.
        /// </summary>
        /// <param name="packageId">The unique identifier of the Freelance PT Package to delete.</param>
        /// <returns>A success response if the deletion is successful.</returns>
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