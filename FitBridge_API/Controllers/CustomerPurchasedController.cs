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

    /// <summary>
    /// Use for customer to view list of their purchased packages both FreelancePtPackage and GymCourse
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [HttpGet("customer-package")]
    public async Task<IActionResult> GetCustomerPurchasedPackage([FromQuery] GetCustomerPurchasedParams parameters)
    {
        var response = await _mediator.Send(new GetCustomerPurchasedPackage { Params = parameters });
        var result = new
        {
            FreelancePtPackage = ResultWithPagination(response.freelancePtPackages.Items, response.freelancePtPackages.Total, parameters.Page, parameters.Size),
            GymCourse = ResultWithPagination(response.gymCourses.Items, response.gymCourses.Total, parameters.Page, parameters.Size),
        };
        return Ok(new BaseResponse<object>(StatusCodes.Status200OK.ToString(), "Get customer purchased freelance pt success", result));
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
    /// Check if a customer and PT have an active customer purchased package.
    /// This API is used to validate booking requests in chat box and other scenarios.
    /// </summary>
    /// <param name="PtId">The PT ID to check (optional - used when customer is checking)</param>
    /// <param name="CustomerId">The Customer ID to check (optional - used when PT is checking)</param>
    /// <returns>On success, returns the CustomerPurchasedId</returns>
    /// <remarks>
    /// This endpoint supports two usage scenarios:
    ///
    /// **Scenario 1: Customer checking if they have a package with a PT**
    /// - Provide only PtId parameter
    /// - The customer is identified from the authentication token
    /// - Returns the CustomerPurchased record if an active package exists
    ///
    /// **Scenario 2: PT checking if a customer has a package with them**
    /// - Provide only CustomerId parameter
    /// - The PT is identified from the authentication token
    /// - Returns the CustomerPurchased record if an active package exists
    ///
    /// Sample requests:
    ///
    ///     GET /api/v1/customer-purchased/check?ptId=3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///     (Customer checking for package with PT)
    ///
    ///     GET /api/v1/customer-purchased/check?customerId=3fa85f64-5717-4562-b3fc-2c963f66afa7
    ///     (PT checking for customer's package)
    ///
    /// </remarks>
    /// <response code="200">Returns the CustomerPurchasedId</response>
    /// <response code="400">Neither PtId nor CustomerId was provided</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Customer purchased package not found or has expired</response>
    [HttpGet("check")]
    [ProducesResponseType(typeof(BaseResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CheckCustomerPurchased([FromQuery] Guid? PtId, [FromQuery] Guid? CustomerId)
    {
        var command = new CheckCustomerPurchasedCommand { PtId = PtId, CustomerId = CustomerId };
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