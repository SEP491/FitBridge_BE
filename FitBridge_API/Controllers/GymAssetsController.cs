using System.Security.Claims;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.GymAssets;
using FitBridge_Application.Features.GymAssets.CreateGymAsset;
using FitBridge_Application.Features.GymAssets.DeleteGymAsset;
using FitBridge_Application.Features.GymAssets.GetGymAssetMetadata;
using FitBridge_Application.Features.GymAssets.GetGymAssets;
using FitBridge_Application.Features.GymAssets.UpdateGymAsset;
using FitBridge_Application.Specifications.GymAssets.GetGymAssetMetadata;
using FitBridge_Application.Specifications.GymAssets.GetGymAssets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers;

/// <summary>
/// Controller for managing gym assets (facilities and equipment)
/// </summary>
public class GymAssetsController(IMediator mediator) : _BaseApiController
{
    /// <summary>
    /// Creates a new gym asset for the authenticated gym owner
    /// </summary>
    /// <param name="command">Asset creation details including asset metadata ID and quantity</param>
    /// <returns>The ID of the newly created gym asset</returns>
    /// <response code="200">Gym asset created successfully</response>
    /// <response code="400">Invalid request data or quantity</response>
    /// <response code="401">Unauthorized - User must be authenticated</response>
    /// <response code="403">Forbidden - Only Gym Owners can create assets</response>
    /// <response code="404">Asset metadata not found</response>
    [HttpPost]
    [Authorize(Roles = ProjectConstant.UserRoles.GymOwner)]
    [ProducesResponseType(typeof(BaseResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateGymAsset([FromForm] CreateGymAssetCommand command)
    {
        var result = await mediator.Send(command);

        return Ok(new BaseResponse<Guid>(
            StatusCodes.Status200OK.ToString(),
            "Gym asset created successfully",
            result));
    }

    /// <summary>
    /// Retrieves gym assets with optional filtering
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and pagination</param>
    /// <returns>Paginated list of gym assets</returns>
    /// <remarks>
    /// Query parameters:
    /// - gymOwnerId: Filter by gym owner (optional)
    /// - assetMetadataId: Filter by asset type (optional)
    /// - assetType: Filter by asset type (Facility/Equipment) (optional)
    /// - page: Page number (default: 1)
    /// - size: Items per page (default: 10)
    /// - sortOrder: Sort order (asc/desc, default: desc)
    /// 
    /// Example: GET /api/v1/gym-assets?gymOwnerId=3fa85f64-5717-4562-b3fc-2c963f66afa6&amp;assetType=Equipment&amp;page=1&amp;size=10
    /// </remarks>
    /// <response code="200">Gym assets retrieved successfully</response>
    /// <response code="400">Invalid query parameters</response>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<Pagination<GymAssetResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetGymAssets([FromQuery] GetGymAssetsParams parameters)
    {
        var query = new GetGymAssetsQuery { Params = parameters };
        var result = await mediator.Send(query);

        var pagination = ResultWithPagination(
            result.Items,
            result.Total,
            parameters.Page,
            parameters.Size);

        return Ok(new BaseResponse<Pagination<GymAssetResponseDto>>(
            StatusCodes.Status200OK.ToString(),
            "Gym assets retrieved successfully",
            pagination));
    }

    /// <summary>
    /// Updates an existing gym asset (quantity and/or images)
    /// </summary>
    /// <param name="id">The ID of the gym asset to update</param>
    /// <param name="command">Update details including quantity and images to add/remove</param>
    /// <returns>Confirmation of update</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/v1/gym-assets/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///     Content-Type: multipart/form-data
    ///
    ///     quantity: 10
    ///     imagesToRemove: ["https://storage.com/old-image-1.jpg", "https://storage.com/old-image-2.jpg"]
    ///     imagesToAdd: [file1.jpg, file2.jpg]
    ///
    /// Requirements:
    /// - User must be the gym owner who created the asset
    /// - Follows strict "Remove, then Add" pattern for images
    /// - Can update quantity, images, or both
    /// </remarks>
    /// <response code="200">Gym asset updated successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">Unauthorized - User must be authenticated</response>
    /// <response code="403">Forbidden - Only the asset owner can update it</response>
    /// <response code="404">Gym asset not found</response>
    [HttpPut]
    [Authorize(Roles = ProjectConstant.UserRoles.GymOwner)]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateGymAsset([FromForm] UpdateGymAssetCommand command)
    {
        var result = await mediator.Send(command);

        return Ok(new BaseResponse<bool>(
            StatusCodes.Status200OK.ToString(),
            "Gym asset updated successfully",
            result));
    }

    /// <summary>
    /// Deletes a gym asset
    /// </summary>
    /// <param name="id">The ID of the gym asset to delete</param>
    /// <returns>Confirmation of deletion</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /api/v1/gym-assets/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///
    /// Requirements:
    /// - User must be the gym owner who created the asset
    /// - Performs soft delete (sets IsEnabled to false)
    /// - Deletes associated images from storage
    /// </remarks>
    /// <response code="200">Gym asset deleted successfully</response>
    /// <response code="401">Unauthorized - User must be authenticated</response>
    /// <response code="403">Forbidden - Only the asset owner can delete it</response>
    /// <response code="404">Gym asset not found</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = ProjectConstant.UserRoles.GymOwner)]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGymAsset([FromRoute] Guid id)
    {
        var command = new DeleteGymAssetCommand
        {
            GymAssetId = id
        };

        var result = await mediator.Send(command);

        return Ok(new BaseResponse<bool>(
            StatusCodes.Status200OK.ToString(),
            "Gym asset deleted successfully",
            result));
    }
    
    [HttpGet("metadata")]
    public async Task<IActionResult> GetGymAssetMetadata([FromQuery] GetGymAssetMetadataParams metadataParams)
    {
        var result = await mediator.Send(new GetGymAssetMetadataQuery { Params = metadataParams });
        var pagination = ResultWithPagination(result.Items, result.Total, metadataParams.Page, metadataParams.Size);
        return Ok(new BaseResponse<Pagination<AssetMetadataBrief>>(
            StatusCodes.Status200OK.ToString(),
            "Gym asset metadata retrieved successfully",
            pagination));
    }
}
