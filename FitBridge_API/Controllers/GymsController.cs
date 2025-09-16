#region Imports

using FitBridge_API.Helpers;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Features.Gym.Queries.GetAllGyms;
using FitBridge_Application.Features.Gym.Queries.GetGymDetails;
using FitBridge_Application.Specifications.Gym;
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
    /// All endpoints return JSON wrapped in a BaseResponse object.
    /// </summary>
    [Produces(MediaTypeNames.Application.Json)]
    public class GymsController(IMediator mediator) : _BaseApiController
    {
        /// <summary>
        /// Retrieves gym details by its identifier.
        /// </summary>
        /// <param name="gymId">The unique identifier of the gym.</param>
        /// <returns>
        /// 200: Returns a BaseResponse containing GetGymDetailsDto when the gym is found.
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
        /// Paging is enabled by default. Use the 'DoApplyPaging' parameter to disable it. With paging enabled, page size = 10 by default, page number starts at 1.
        ///
        /// Filter options :
        /// - gymName: partial / case-insensitive text match against the gym name.
        ///
        /// Ordering (SortBy / SortDirection):
        /// - gymName / asc (default) | desc
        /// - representName / asc (default) | desc
        ///
        /// </remarks>
        /// <param name="getAllGymsParams">Query parameters for paging, filtering and sorting gyms. Supported fields typically include: Page, Size, Search, SortBy, SortDirection, Latitude, Longitude, Radius, HotResearch.</param>
        /// <returns>
        /// 200: Returns a BaseResponse containing a Pagination object with GetAllGymsDto items.
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
    }
}