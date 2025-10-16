using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Membership;
using FitBridge_Application.Features.Memberships.CreateMembership;
using FitBridge_Application.Features.Memberships.DeleteMembership;
using FitBridge_Application.Features.Memberships.GetAllMemberships;
using FitBridge_Application.Features.Memberships.GetMembershipById;
using FitBridge_Application.Features.Memberships.UpdateMembership;
using FitBridge_Application.Specifications.Memberships.GetAllMemberships;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers
{
    /// <summary>
    /// Controller for managing memberships/service information, including creation, retrieval, update, and deletion.
    /// </summary>
    [Authorize]
    public class MembershipsController(IMediator mediator) : _BaseApiController
    {
        /// <summary>
        /// Retrieves a paginated list of all memberships.
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination, including:
        /// <list type="bullet">
        /// <item>
        /// <term>Page</term>
        /// <description>The page number to retrieve.</description>
        /// </item>
        /// <item>
        /// <term>Size</term>
        /// <description>The number of items per page.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>A paginated list of memberships.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<Pagination<GetMembershipDto>>))]
        public async Task<IActionResult> GetMemberships([FromQuery] GetAllMembershipsParam parameters)
        {
            var result = await mediator.Send(new GetAllMembershipsQuery { Params = parameters });
            var pagination = ResultWithPagination(result.Items, result.Total, parameters.Page, parameters.Size);
            return Ok(
                new BaseResponse<Pagination<GetMembershipDto>>(
                    StatusCodes.Status200OK.ToString(),
                    "Memberships retrieved successfully",
                    pagination));
        }

        /// <summary>
        /// Retrieves a specific membership by its unique identifier.
        /// </summary>
        /// <param name="membershipId">The unique identifier (GUID) of the membership to retrieve:
        /// <list type="bullet">
        /// <item>
        /// <term>Id</term>
        /// <description>The unique identifier of the membership. Only active memberships are returned.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>The membership details including service name, charge, and research slot information.</returns>
        [HttpGet("{membershipId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<GetMembershipDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMembershipById([FromRoute] Guid membershipId)
        {
            var result = await mediator.Send(new GetMembershipByIdQuery { MembershipId = membershipId });
            return Ok(
                new BaseResponse<GetMembershipDto>(
                    StatusCodes.Status200OK.ToString(),
                    "Membership retrieved successfully",
                    result));
        }

        /// <summary>
        /// Creates a new membership with the specified details.
        /// </summary>
        /// <param name="command">The details of the membership to create, including:
        /// <list type="bullet">
        /// <item>
        /// <term>ServiceName</term>
        /// <description>The name of the service/membership.</description>
        /// </item>
        /// <item>
        /// <term>ServiceCharge</term>
        /// <description>The charge amount for the service.</description>
        /// </item>
        /// <item>
        /// <term>MaximumHotResearchSlot</term>
        /// <description>The maximum number of hot research slots available.</description>
        /// </item>
        /// <item>
        /// <term>AvailableHotResearchSlot</term>
        /// <description>The currently available hot research slots.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>The created membership details.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BaseResponse<CreateMembershipDto>))]
        public async Task<IActionResult> CreateMembership([FromBody] CreateMembershipCommand command)
        {
            var membershipDto = await mediator.Send(command);
            return Created(
                nameof(CreateMembership),
                new BaseResponse<CreateMembershipDto>(
                    StatusCodes.Status201Created.ToString(),
                    "Membership created successfully",
                    membershipDto));
        }

        /// <summary>
        /// Updates an existing membership with the specified ID.
        /// </summary>
        /// <param name="membershipId">The unique identifier of the membership to update.</param>
        /// <param name="updateMembershipCommand">The updated details of the membership, including:
        /// <list type="bullet">
        /// <item>
        /// <term>ServiceName</term>
        /// <description>The updated name of the service/membership.</description>
        /// </item>
        /// <item>
        /// <term>ServiceCharge</term>
        /// <description>The updated charge amount for the service.</description>
        /// </item>
        /// <item>
        /// <term>MaximumHotResearchSlot</term>
        /// <description>The updated maximum number of hot research slots.</description>
        /// </item>
        /// <item>
        /// <term>AvailableHotResearchSlot</term>
        /// <description>The updated number of available hot research slots.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>The updated membership details.</returns>
        [HttpPut("{membershipId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<UpdateMembershipDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMembership([FromRoute] Guid membershipId, [FromBody] UpdateMembershipCommand updateMembershipCommand)
        {
            updateMembershipCommand.MembershipId = membershipId;
            var updatedMembership = await mediator.Send(updateMembershipCommand);
            return Ok(
                new BaseResponse<UpdateMembershipDto>(
                    StatusCodes.Status200OK.ToString(),
                    "Membership updated successfully",
                    updatedMembership));
        }

        /// <summary>
        /// Soft deletes a membership with the specified ID.
        /// </summary>
        /// <param name="membershipId">The unique identifier of the membership to delete.</param>
        /// <returns>A success response if the deletion is successful.</returns>
        [HttpDelete("{membershipId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<EmptyResult>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMembership([FromRoute] Guid membershipId)
        {
            await mediator.Send(new DeleteMembershipCommand { MembershipId = membershipId });
            return Ok(
                new BaseResponse<EmptyResult>(
                    StatusCodes.Status200OK.ToString(),
                    "Membership deleted successfully",
                    Empty));
        }
    }
}
