using System;
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Application.Features.GymSlots.CreateGymSlot;
using FitBridge_API.Helpers.RequestHelpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FitBridge_Application.Features.GymSlots.DeleteGymSlotById;
using FitBridge_Application.Features.GymSlots.UpdateGymSlot;
using FitBridge_Application.Specifications.GymSlots;
using FitBridge_Application.Features.GymSlots.GetAllGymSlot;
using FitBridge_Application.Features.GymSlots.RegisterSlot;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Features.GymSlots.DeactivateSlot;
using FitBridge_Application.Features.GymSlots.CustomerRegisterSlot;
using FitBridge_Application.Features.GymSlots.CheckMinimumSlot;
using FitBridge_Application.Features.GymSlots.GetAllPtSlot;
using FitBridge_Application.Dtos;
using FitBridge_Application.Features.GymSlots.GetGymPtSchedule;

namespace FitBridge_API.Controllers;

public class GymSlotsController(IMediator _mediator) : _BaseApiController
{
    [HttpPost]
    [Authorize(Roles = "GymOwner, Admin")]
    public async Task<IActionResult> CreateGymSlot([FromBody] CreateNewSlotResponse request)
    {
        var result = await _mediator.Send(new CreateGymSlotCommand { Request = request });
        return Ok(new BaseResponse<CreateNewSlotResponse>(StatusCodes.Status200OK.ToString(), "Gym slot created successfully", result));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "GymOwner, Admin")]
    public async Task<IActionResult> DeleteGymSlot(string id)
    {
        var result = await _mediator.Send(new DeleteGymSlotByIdCommand(id));
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Gym slot deleted successfully", result));
    }

    [HttpPut]
    [Authorize(Roles = "GymOwner, Admin")]
    public async Task<IActionResult> UpdateGymSlot([FromBody] UpdateGymSlotCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<SlotResponseDto>(StatusCodes.Status200OK.ToString(), "Gym slot updated successfully", result));
    }

    [HttpGet]
    [Authorize(Roles = "GymOwner, Admin")]
    public async Task<IActionResult> GetGymSlots([FromQuery] GetGymSlotParams parameters)
    {
        var result = await _mediator.Send(new GetGymSlotsQuery { Params = parameters });
        var pagination = ResultWithPagination(result.Items, result.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<SlotResponseDto>>(StatusCodes.Status200OK.ToString(), "Gym slots retrieved successfully", pagination));
    }

    /// <summary>
    /// Registers a Personal Trainer (PT) for a specific gym slot on a given date
    /// </summary>
    /// <param name="command">The registration details including slot ID and registration date</param>
    /// <returns>Confirmation of successful registration</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/v1/gymslots/register-slot
    ///     {
    ///         "slotId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "registerDate": "2024-12-25"
    ///     }
    /// 
    /// Only authenticated PTs can register for slots in their assigned gym.
    /// </remarks>
    /// <response code="200">Slot registered successfully</response>
    /// <response code="400">Invalid request data or slot already registered</response>
    /// <response code="401">Unauthorized - User must be authenticated</response>
    /// <response code="403">Forbidden - Only Personal Trainers can register for slots</response>
    /// <response code="404">Gym slot not found</response>
    /// <response code="409">Conflict - Slot already registered by this PT for the date</response>
    [HttpPost("register-slot")]
    [Authorize(Roles = ProjectConstant.UserRoles.GymPT)]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterSlot([FromBody] RegisterSlotCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Slot registered successfully", result));
    }

    /// <summary>
    /// Deactivates a PT's registered slot, making it unavailable for customer bookings
    /// </summary>
    /// <param name="command">The deactivation details including PT gym slot ID</param>
    /// <returns>Confirmation of successful deactivation</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/v1/gymslots/deactivated-slots
    ///     {
    ///         "ptGymSlotId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// 
    /// This action makes the slot unavailable but preserves the registration.
    /// Only the PT who registered for the slot can deactivate it.
    /// </remarks>
    /// <response code="200">Slot deactivated successfully</response>
    /// <response code="400">Invalid PT gym slot ID</response>
    /// <response code="401">Unauthorized - User must be authenticated</response>
    /// <response code="403">Forbidden - Only Personal Trainers can deactivate their slots</response>
    /// <response code="404">PT gym slot not found</response>
    [HttpPost("deactivated-slots")]
    [Authorize(Roles = ProjectConstant.UserRoles.GymPT)]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateSlot([FromBody] DeactivateSlotCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Slot deactivated successfully", result));
    }

    /// <summary>
    /// Allows a customer to book an available PT gym slot
    /// </summary>
    /// <param name="command">The booking details including PT gym slot ID and customer purchased package ID</param>
    /// <returns>Confirmation of successful booking</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/v1/gymslots/customer-register-slot
    ///     {
    ///         "ptGymSlotId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "customerPurchasedId": "2fa85f64-5717-4562-b3fc-2c963f66afa7"
    ///     }
    /// 
    /// Requirements:
    /// - Customer must have available sessions in their purchased package
    /// - PT gym slot must be activated and available
    /// - No time conflicts with existing bookings
    /// </remarks>
    /// <response code="200">Slot registered successfully for customer</response>
    /// <response code="400">Invalid request data, insufficient sessions, or time conflict</response>
    /// <response code="401">Unauthorized - User must be authenticated</response>
    /// <response code="403">Forbidden - Only Customers can book slots</response>
    /// <response code="404">PT gym slot or customer package not found</response>
    /// <response code="409">Conflict - Slot already booked or time overlap</response>
    [HttpPost("customer-register-slot")]
    [Authorize(Roles = ProjectConstant.UserRoles.Customer)]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status409Conflict)]
    [Authorize(Roles = ProjectConstant.UserRoles.Customer)]
    public async Task<IActionResult> CustomerRegisterSlot([FromBody] CustomerRegisterSlotCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Slot registered successfully", result));
    }
    /// <summary>
    /// Checks if a PT has registered the minimum required slots for a given week period
    /// </summary>
    /// <param name="command">The check parameters including start and end week dates</param>
    /// <returns>Information about minimum slot requirements and current registrations</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/v1/gymslots/check-minimum-slot
    ///     {
    ///         "startWeek": "2024-12-23",
    ///         "endWeek": "2024-12-29"
    ///     }
    /// 
    /// Returns:
    /// - Minimum required slots for the period
    /// - Currently registered slots
    /// - Whether the requirement is met
    /// </remarks>
    /// <response code="200">Slot check completed successfully</response>
    /// <response code="400">Invalid date range or parameters</response>
    /// <response code="401">Unauthorized - User must be authenticated</response>
    [HttpPost("check-minimum-slot")]
    [ProducesResponseType(typeof(BaseResponse<CheckMinimumSlotResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CheckMinimumSlot([FromBody] CheckMinimumSlotCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<CheckMinimumSlotResponseDto>(StatusCodes.Status200OK.ToString(), "Slot check successfully", result));
    }

    /// <summary>
    /// Retrieves all slots registered by a specific PT for a given date
    /// </summary>
    /// <param name="parameters">Query parameters including PT ID, registration date, and pagination options</param>
    /// <returns>Paginated list of PT's registered slots with booking information</returns>
    /// <remarks>
    /// Query parameters:
    /// - ptId: Personal Trainer's unique identifier (required)
    /// - registerDate: Date for which to retrieve slots (required, format: YYYY-MM-DD)
    /// - page: Page number (default: 1)
    /// - size: Items per page (default: 10)
    /// 
    /// Example: GET /api/v1/gymslots/all-pt-slots?ptId=3fa85f64-5717-4562-b3fc-2c963f66afa6&amp;registerDate=2024-12-25
    /// </remarks>
    /// <response code="200">Slots retrieved successfully</response>
    /// <response code="400">Invalid PT ID or date format</response>
    /// <response code="401">Unauthorized - User must be authenticated</response>
    /// <response code="404">PT not found</response>
    [HttpGet("all-pt-slots")]
    [ProducesResponseType(typeof(BaseResponse<Pagination<GetPTSlot>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllPtSlots([FromQuery] GetAllPtSlotsParams parameters)
    {
        var result = await _mediator.Send(new GetAllPtSlotsQuery { Params = parameters });
        var pagination = ResultWithPagination(result.Items, result.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<GetPTSlot>>(StatusCodes.Status200OK.ToString(), "Slots retrieved successfully", pagination));
    }

    [HttpGet("gym-pt-schedule")]
    public async Task<IActionResult> GetGymPtSchedule([FromQuery] GetGymPtScheduleParams parameters)
    {
        var result = await _mediator.Send(new GetGymPtScheduleQuery { Params = parameters });
        var pagination = ResultWithPagination(result.Items, result.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<PTSlotScheduleResponse>>(StatusCodes.Status200OK.ToString(), "Schedule retrieved successfully", pagination));
    }
}
