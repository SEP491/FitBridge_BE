using System;
using FitBridge_API.Helpers;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.CustomerPurchaseds;
using FitBridge_Application.Dtos.GymCourses;
using FitBridge_Application.Features.CustomerPurchaseds.CheckCustomerPurchased;
using FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchased;
using FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchasedByCustomerId;
using FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchasedFreelancePt;
using FitBridge_Application.Features.GymCourses.GetPurchasedGymCoursePtForSchedule;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByCustomerId;
using FitBridge_Application.Specifications.GymCoursePts.GetPurchasedGymCoursePtForScheduleGetPurchasedGymCoursePtForSchedule;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers;

public class CustomerPurchasedController(IMediator _mediator) : _BaseApiController
{
    [HttpGet("customer-schedule")]
    public async Task<IActionResult> GetPurchasedGymCoursePtForSchedule([FromQuery] GetPurchasedGymCoursePtForScheduleParams parameters)
    {
        var response = await _mediator.Send(new GetPurchasedGymCoursePtForScheduleQuery { Params = parameters });
        var pagination = ResultWithPagination(response.Items, response.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<GymCoursesPtResponse>>(StatusCodes.Status200OK.ToString(), "Get purchased gym course pt for schedule success", pagination));
    }

    [HttpGet("customer-package/gym-course")]
    public async Task<IActionResult> GetCustomerPurchasedGymCourse([FromQuery] GetCustomerPurchasedParams parameters)
    {
        var response = await _mediator.Send(new GetCustomerPurchasedQuery { Params = parameters });
        var pagination = ResultWithPagination(response.Items, response.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<CustomerPurchasedResponseDto>>(StatusCodes.Status200OK.ToString(), "Get customer purchased course success", pagination));
    }

    [HttpGet("customer-package/freelance-pt")]
    public async Task<IActionResult> GetCustomerPurchasedFreelancePt([FromQuery] GetCustomerPurchasedParams parameters)
    {
        var response = await _mediator.Send(new GetCustomerPurchasedFreelancePtQuery { Params = parameters });
        var pagination = ResultWithPagination(response.Items, response.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<CustomerPurchasedFreelancePtResponseDto>>(StatusCodes.Status200OK.ToString(), "Get customer purchased freelance pt success", pagination));
    }

    /// <summary>
    /// Get customer purchased packages by customer ID
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <param name="parameters">Query parameters for pagination and filtering</param>
    /// <returns>Returns a paginated list of customer purchased freelance PT packages</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/v1/customer-purchased/customer/{customerId}?page=1&amp;size=10&amp;isOngoingOnly=true
    ///
    /// This endpoint retrieves all purchased packages for a specific customer, including:
    /// - Package name and image
    /// - Available sessions remaining
    /// - Expiration date
    /// - Freelance PT package ID (if applicable)
    ///
    /// Use the `isOngoingOnly` parameter to filter for only active/ongoing packages.
    /// </remarks>
    /// <response code="200">Customer purchased packages retrieved successfully</response>
    /// <response code="404">Customer not found</response>
    [HttpGet("customer/{customerId}")]
    [ProducesResponseType(typeof(BaseResponse<Pagination<CustomerPurchasedFreelancePtResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerPurchasedByCustomerId(
        [FromRoute] Guid customerId, 
        [FromQuery] GetCustomerPurchasedParams parameters)
    {
        var query = new GetCustomerPurchasedByCustomerIdQuery(parameters)
        {
            CustomerId = customerId
        };
        
        var response = await _mediator.Send(query);
        var pagination = ResultWithPagination(response.Items, response.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<CustomerPurchasedFreelancePtResponseDto>>(
            StatusCodes.Status200OK.ToString(), 
            "Customer purchased packages retrieved successfully", 
            pagination));
    }

    /// <summary>
    /// Check if the customer and this pt already have a customer purchased package, this api use for validate booking request in chat box
    /// </summary>
    /// <param name="PtId"></param>
    /// <returns>On success, return the CustomerPurchasedId</returns>
    [HttpGet("check")]
    public async Task<IActionResult> CheckCustomerPurchased([FromQuery] Guid PtId)
    {
        var command = new CheckCustomerPurchasedCommand { PtId = PtId };
        var response = await _mediator.Send(command);
        return Ok(new BaseResponse<Guid>(StatusCodes.Status200OK.ToString(), "Check customer purchased success", response));
    }
}
