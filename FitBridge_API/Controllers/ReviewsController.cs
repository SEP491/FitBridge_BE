using System;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Reviews;
using FitBridge_Application.Features.Reviews.CreateReview;
using FitBridge_Application.Specifications.Reviews.GetAllReview;
using FitBridge_Application.Features.Reviews.GetAllReview;
using FitBridge_Application.Features.Reviews.UpdateReview;
using FitBridge_Application.Features.Reviews.DeleteReview;
using FitBridge_Application.Specifications.Reviews.GetAllReviewForAdmin;
using FitBridge_Application.Features.Reviews.GetAllReviewForAdmin;
using FitBridge_Application.Specifications.Reviews.GetAllReviewForCustomer;
using FitBridge_Application.Features.Reviews.GetCustomerReviews;
namespace FitBridge_API.Controllers;

public class ReviewsController(IMediator mediator) : _BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateReview([FromForm] CreateReviewCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(new BaseResponse<ReviewProductResponseDto>(StatusCodes.Status200OK.ToString(), "Review created successfully", result));
    }

    [HttpGet("feedback-target")]
    public async Task<IActionResult> GetAllReviewByTargetId([FromQuery] GetAllReviewQueryParams queryParams)
    {
        var result = await mediator.Send(new GetAllReviewByTargetIdQuery { Params = queryParams });
        var pagination = ResultWithPagination(result.Items, result.Total, queryParams.Page, queryParams.Size);
        return Ok(new BaseResponse<Pagination<ReviewProductResponseDto>>(StatusCodes.Status200OK.ToString(), "Reviews retrieved successfully", pagination));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateReview([FromForm] UpdateReviewCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(new BaseResponse<ReviewProductResponseDto>(StatusCodes.Status200OK.ToString(), "Review updated successfully", result));
    }

    [HttpDelete("{reviewId}")]
    public async Task<IActionResult> DeleteReview([FromRoute] Guid reviewId)
    {
        var result = await mediator.Send(new DeleteReviewCommand { ReviewId = reviewId });
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Review deleted successfully", result));
    }

    /// <summary>
    /// Get all reviews for admin, search term search by content, sort by created at asc or desc
    /// </summary>
    /// <param name="queryParams">Query parameters for pagination and filtering</param>
    /// <returns>A paginated list of reviews</returns>
    [HttpGet("admin/all")]
    public async Task<IActionResult> GetAllReviews([FromQuery] GetAllReviewsForAdminQueryParams queryParams)
    {
        var result = await mediator.Send(new GetAllReviewsForAdminQuery { Params = queryParams });
        return Ok(new BaseResponse<List<ReviewProductResponseDto>>(StatusCodes.Status200OK.ToString(), "Reviews retrieved successfully", result));
    }

    [HttpGet("customer")]
    public async Task<IActionResult> GetAllCustomerReviews([FromQuery] GetCustomerReviewParams parameters)
    {
        var result = await mediator.Send(new GetCustomerReviewQuery(parameters));
        var pagination = ResultWithPagination(result.Items, result.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<UserReviewResponseDto>>(StatusCodes.Status200OK.ToString(), "Customer reviews retrieved successfully", pagination));
    }
}
