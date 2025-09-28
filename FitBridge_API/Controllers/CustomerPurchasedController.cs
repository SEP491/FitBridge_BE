using System;
using FitBridge_API.Helpers;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.CustomerPurchaseds;
using FitBridge_Application.Dtos.GymCourses;
using FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchased;
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
}
