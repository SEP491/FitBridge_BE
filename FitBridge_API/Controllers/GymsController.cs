#region Imports

using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Features.Gym.Queries.GetGymDetails;
using MediatR;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace FitBridge_API.Controllers
{
    public class GymsController(IMediator mediator) : _BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<GetGymDetailsDto>> GetGymDetailsById([FromQuery] Guid gymId)
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
    }
}