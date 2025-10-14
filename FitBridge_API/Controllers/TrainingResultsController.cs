using System;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.TrainingResults;
using FitBridge_Application.Features.TrainingResults.GetPackageTrainingResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers;

public class TrainingResultsController(IMediator _mediator) : _BaseApiController
{
    /// <summary>
    /// Get comprehensive analytics and metrics for a customer purchased package
    /// </summary>
    /// <param name="customerPurchasedId">The ID of the customer purchased package</param>
    /// <returns>Detailed analytics including session statistics, workout metrics, and muscle group breakdown</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/v1/training-results/analytics/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///
    /// Returns comprehensive metrics including:
    /// - Session statistics (total, completed, cancelled, upcoming)
    /// - Activity completion rates
    /// - Average metrics (time, weight, sets per session)
    /// - Highest performance (best session by weight lifted)
    /// - Muscle group insights (most/least trained)
    /// - Workout statistics (weight lifted, reps, practice time)
    /// - Muscle group breakdown
    /// </remarks>
    [HttpGet("analytics/{customerPurchasedId}")]
    [Authorize]
    public async Task<IActionResult> GetCustomerPurchasedAnalytics([FromRoute] Guid customerPurchasedId)
    {
        var result = await _mediator.Send(new GetPackageTrainingResultsQuery 
        { 
            CustomerPurchasedId = customerPurchasedId 
        });
        return Ok(new BaseResponse<CustomerPurchasedAnalyticsDto>(
            StatusCodes.Status200OK.ToString(), 
            "Analytics retrieved successfully", 
            result));
    }
}
