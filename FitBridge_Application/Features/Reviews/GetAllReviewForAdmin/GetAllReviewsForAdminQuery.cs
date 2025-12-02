using System;
using FitBridge_Application.Dtos.Reviews;
using FitBridge_Application.Specifications.Reviews.GetAllReviewForAdmin;
using MediatR;

namespace FitBridge_Application.Features.Reviews.GetAllReviewForAdmin;

public class GetAllReviewsForAdminQuery : IRequest<List<ReviewProductResponseDto>>
{
    public GetAllReviewsForAdminQueryParams Params { get; set; }
}
