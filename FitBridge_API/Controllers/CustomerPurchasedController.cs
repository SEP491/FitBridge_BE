using System;
using FitBridge_API.Helpers;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.CustomerPurchaseds;
using FitBridge_Application.Dtos.FreelancePTPackages;
using FitBridge_Application.Dtos.GymCourses;
using FitBridge_Application.Features.CustomerPurchaseds.CheckCustomerPurchased;
using FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchased;
using FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchasedByFreelancePtId;
using FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchasedByCustomerId;
using FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchasedFreelancePt;
using FitBridge_Application.Features.GymCourses.GetPurchasedGymCoursePtForSchedule;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByCustomerId;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedForFreelancePt;
using FitBridge_Application.Specifications.GymCoursePts.GetPurchasedGymCoursePtForScheduleGetPurchasedGymCoursePtForSchedule;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchasedOverallTrainingResults;
using FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchasedTrainingResultsDetails;

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

    /// <summary>
    /// Use for freelance pt to view list of customer that purchased their package and still not expired
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("freelance-pt")]
    public async Task<IActionResult> GetCustomerPurchasedByFreelancePtId([FromQuery] GetCustomerPurchasedForFreelancePtParams parameters)
    {
        var response = await _mediator.Send(new GetCustomerPurchasedByFreelancePtIdQuery { Params = parameters });
        var pagination = ResultWithPagination(response.Items, response.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<GetCustomerPurchasedForFreelancePt>>(StatusCodes.Status200OK.ToString(), "Get customer purchased freelance pt by id success", pagination));
    }

    /// <summary>
    /// Get training results for a purchased freelance PT package
    /// </summary>
    /// <param name="customerPurchasedId">The ID of the purchased package</param>
    [HttpGet("result/{customerPurchasedId}")]
    public async Task<IActionResult> GetPackageTrainingResults([FromRoute] Guid customerPurchasedId)
    {
        var result = await _mediator.Send(new GetCustomerPurchasedOverallTrainingResultsQuery { CustomerPurchasedId = customerPurchasedId });
        return Ok(new BaseResponse<CustomerPurchasedOverallResultResponseDto>(
            StatusCodes.Status200OK.ToString(),
            "Training results retrieved successfully",
            result));
    }

    /// <summary>
    /// Get detailed training results for a purchased freelance PT package
    /// </summary>
    /// <param name="customerPurchasedId">The ID of the purchased package</param>
    [HttpGet("result/{customerPurchasedId}/detail")]
    public async Task<IActionResult> GetTrainingResultsDetails([FromRoute] Guid customerPurchasedId)
    {
        var result = await _mediator.Send(new GetCustomerPurchasedTrainingResultsDetailsQuery { CustomerPurchasedId = customerPurchasedId });
        return Ok(new BaseResponse<CustomerPurchasedTrainingResultsDetailResponseDto>(
            StatusCodes.Status200OK.ToString(),
            "Daily training results retrieved successfully",
            result));
    }
}