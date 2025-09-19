using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Dtos.GymCourses;
using FitBridge_Application.Features.GymCourses.AssignPtToCourse;
using FitBridge_Application.Features.GymCourses.CreateGymCourse;
using FitBridge_Application.Features.GymCourses.GetGymCoursesByGymId;
using FitBridge_Application.Features.Gyms.GetGymPtsByCourse;
using FitBridge_Application.Specifications.Gym.GetGymCoursesByGymId;
using FitBridge_Application.Specifications.Gym.GetGymPtsByCourse;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitBridge_API.Controllers
{
    /// <summary>
    /// Handles gym course related endpoints: retrieving courses by gym, retrieving PTs for a course,
    /// creating gym courses (requires authenticated gym owner), and assigning PTs to courses.
    /// </summary>
    /// <remarks>
    /// Endpoints:
    /// - GET  api/v{version}/GymCourses/{gymId}            : Get paginated gym courses for a gym.
    /// - GET  api/v{version}/GymCourses/{courseId}/pts     : Get paginated PTs for a course.
    /// - POST api/v{version}/GymCourses                    : Create a new gym course (authenticated owner).
    /// - POST api/v{version}/GymCourses/assign-pt-to-course: Assign a PT to a course.
    /// </remarks>
    public class GymCoursesController(IMediator mediator) : _BaseApiController
    {
        /// <summary>
        /// Retrieves paginated gym courses for a specific gym.
        /// </summary>
        /// <param name="gymId">The unique identifier of the gym.</param>
        /// <param name="getGymCourseByGymIdParams">Query parameters for paging and filtering gym courses.</param>
        /// <returns>
        /// An <see cref="ActionResult{Pagination{GetGymCourseDto}}"/> containing paginated gym courses and pagination metadata.
        /// Returns HTTP 200 with the paginated result.
        /// </returns>
        [HttpGet("{gymId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<Pagination<GetGymCourseDto>>))]
        public async Task<ActionResult<Pagination<GetGymPtsDto>>> GetGymCourseByGymId([FromRoute] Guid gymId, [FromQuery] GetGymCourseByGymIdParams getGymCourseByGymIdParams)
        {
            var response = await mediator.Send(new GetGymCoursesByGymIdQuery(gymId, getGymCourseByGymIdParams));

            var pagedResult = new Pagination<GetGymCourseDto>(
                response.Items,
                response.Total,
                getGymCourseByGymIdParams.Page,
                getGymCourseByGymIdParams.Size
                );
            return Ok(
                new BaseResponse<Pagination<GetGymCourseDto>>(
                    StatusCodes.Status200OK.ToString(),
                    "Get gym courses success",
                    pagedResult));
        }

        /// <summary>
        /// Retrieves paginated personal trainer (PT) profiles associated with a gym course.
        /// </summary>
        /// <param name="courseId">The unique identifier of the course. Bound from route.</param>
        /// <param name="getGymPtsParam">Query parameters for paging and filtering PTs. Bound from query.</param>
        /// <returns>
        /// An <see cref="ActionResult{Pagination{GetGymPtsDto}}"/> containing paginated PT profiles and pagination metadata.
        /// Returns HTTP 200 with the paginated result.
        /// </returns>
        [HttpGet("{courseId}/pts")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<Pagination<GetGymPtsDto>>))]
        public async Task<ActionResult<Pagination<GetGymPtsDto>>> GetGymPtsByGymCourse([FromRoute] Guid courseId, [FromQuery] GetGymPtsByGymCourseParams getGymPtsParam)
        {
            var response = await mediator.Send(new GetGymPtsByCourseQuery(getGymPtsParam, courseId));

            var pagedResult = new Pagination<GetGymPtsDto>(
                response.Items,
                response.Total,
                getGymPtsParam.Page,
                getGymPtsParam.Size);
            return Ok(
                new BaseResponse<Pagination<GetGymPtsDto>>(
                    StatusCodes.Status200OK.ToString(),
                    "Get gym pts success",
                    pagedResult));
        }

        /// <summary>
        /// Creates a new gym course.
        /// </summary>
        /// <param name="command">The command containing gym course details.</param>
        /// <returns>
        /// An <see cref="ActionResult{CreateGymCourseResponse}"/> containing the created gym course details.
        /// Returns HTTP 200 if successful, or HTTP 400 if the gym owner is not found.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<Pagination<GetGymPtsDto>>> CreateGymCourse([FromBody] CreateGymCourseCommand command)
        {
            command.GymOwnerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            if (string.IsNullOrEmpty(command.GymOwnerId))
            {
                return BadRequest(new BaseResponse<string>(StatusCodes.Status400BadRequest.ToString(), "Gym owner not found", null));
            }
            var response = await mediator.Send(command);
            return Ok(
                new BaseResponse<CreateGymCourseResponse>(
                    StatusCodes.Status200OK.ToString(),
                    "Create gym course success",
                    response));
        }

        /// <summary>
        /// Assigns a personal trainer (PT) to a gym course.
        /// </summary>
        /// <param name="command">The command containing PT and course assignment details.</param>
        /// <returns>
        /// An <see cref="ActionResult{Guid}"/> containing the assignment result.
        /// Returns HTTP 200 with the assignment identifier.
        /// </returns>
        [HttpPost("assign-pt-to-course")]
        public async Task<ActionResult<Pagination<GetGymPtsDto>>> AssignPtToCourse([FromBody] AssignPtToCourseCommand command)
        {
            var response = await mediator.Send(command);
            return Ok(
                new BaseResponse<Guid>(
                    StatusCodes.Status200OK.ToString(),
                    "Assign pt to course success",
                    response));
        }
    }
}