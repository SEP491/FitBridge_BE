#region Imports

using FitBridge_API.Helpers;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Features.Gyms.GetAllGyms;
using FitBridge_Application.Features.Gyms.GetGymDetails;
using FitBridge_Application.Features.Gyms.GetGymPtsByCourse;
using FitBridge_Application.Features.Gyms.GetGymPtsByGymId;
using FitBridge_Application.Specifications.Gym.GetAllGyms;
using FitBridge_Application.Specifications.Gym.GetGymPtsByCourse;
using FitBridge_Application.Specifications.Gym.GetGymPtsByGymId;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Threading.Tasks;

#endregion

namespace FitBridge_API.Controllers
{
    /// <summary>
    /// Controller that exposes endpoints to query gyms.
    /// All endpoints return JSON wrapped in a <see cref="BaseResponse{T}"/> object.
    /// </summary>
    /// <remarks>
    /// Route: api/v{version:apiVersion}/Gyms
    /// This controller provides endpoints to:
    /// - Retrieve gym details by id.
    /// - Retrieve a paginated list of gyms with filtering and sorting.
    /// - Retrieve paginated pts (personal trainers) for a given gym.
    /// </remarks>
    [Produces(MediaTypeNames.Application.Json)]
    public class GymsController(IMediator mediator) : _BaseApiController
    {
        /// <summary>
        /// Retrieves gym details by its identifier.
        /// </summary>
        /// <param name="gymId">The unique identifier of the gym. Bound from route.</param>
        /// <returns>
        /// A <see cref="BaseResponse{GetGymDetailsDto}"/> containing the gym details when found.
        /// Returns HTTP 200 with the data.
        /// </returns>
        [HttpGet("{gymId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<GetGymDetailsDto>))]
        public async Task<ActionResult<GetGymDetailsDto>> GetGymDetailsById([FromRoute] Guid gymId)
        {
            var response = await mediator.Send(new GetGymDetailsByIdQuery
            {
                Id = gymId
            });

            return Ok(
                new BaseResponse<GetGymDetailsDto>(
                    StatusCodes.Status200OK.ToString(),
                    "Get gym details by id success",
                    response));
        }

        /// <summary>
        /// Retrieves a paginated list of gyms. Supports paging, filtering and sorting.
        /// </summary>
        ///
        /// <remarks>
        /// Paging is enabled by default. Use the 'DoApplyPaging' parameter to disable it.
        /// With paging enabled, page size = 10 by default, page number starts at 1.
        ///
        /// Filter options :
        /// - searchTerm: partial / case-insensitive text match against the gym name.
        ///
        /// Ordering (SortBy / SortDirection):
        /// - gymName / asc (default) | desc
        /// - representName / asc (default) | desc
        /// </remarks>
        /// <param name="getAllGymsParams">Query parameters for paging, filtering and sorting gyms. Supported fields typically include: Page, Size, Search, SortBy, SortDirection, Latitude, Longitude, Radius, HotResearch.</param>
        /// <returns>
        /// A <see cref="BaseResponse{Pagination{GetAllGymsDto}}"/> containing paginated gym data and paging metadata.
        /// Returns HTTP 200 with the paginated result.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<Pagination<GetAllGymsDto>>))]
        public async Task<ActionResult<Pagination<GetAllGymsDto>>> GetAllGyms([FromQuery] GetAllGymsParams getAllGymsParams)
        {
            var response = await mediator.Send(new GetAllGymsQuery(getAllGymsParams));

            var pagedResult = new Pagination<GetAllGymsDto>(
                response.Items,
                response.Total,
                getAllGymsParams.Page,
                getAllGymsParams.Size);
            return Ok(
                new BaseResponse<Pagination<GetAllGymsDto>>(
                    StatusCodes.Status200OK.ToString(),
                    "Get all gyms success",
                    pagedResult));
        }

        /// <summary>
        /// Retrieves a paginated list of personal trainers (pts) associated with a specific gym.
        /// </summary>
        /// <param name="gymId">The unique identifier of the gym. Bound from route.</param>
        /// <param name="getGymPtsByGymIdParams">Query parameters for paging, filtering and sorting pts. Typically includes: Page, Size, Search, SortBy, SortDirection.</param>
        /// <returns>
        /// A <see cref="BaseResponse{Pagination{GetGymPtsDto}}"/> containing paginated pts data and paging metadata.
        /// Returns HTTP 200 with the paginated result.
        /// </returns>
        [HttpGet("{gymId}/pts")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<Pagination<GetGymPtsDto>>))]
        public async Task<ActionResult<Pagination<GetGymPtsDto>>> GetGymPtsByGymId([FromRoute] Guid gymId, [FromQuery] GetGymPtsByGymIdParams getGymPtsByGymIdParams)
        {
            var response = await mediator.Send(new GetGymPtsByGymIdQuery(gymId, getGymPtsByGymIdParams));

            var pagedResult = new Pagination<GetGymPtsDto>(
                response.Items,
                response.Total,
                getGymPtsByGymIdParams.Page,
                getGymPtsByGymIdParams.Size);
            return Ok(
                new BaseResponse<Pagination<GetGymPtsDto>>(
                    StatusCodes.Status200OK.ToString(),
                    "Get gym pts success",
                    pagedResult));
        }
    }
}