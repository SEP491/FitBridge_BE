using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Reviews;
using FitBridge_Application.Specifications.Reviews.GetAllReview;
using MediatR;

namespace FitBridge_Application.Features.Reviews.GetAllReview;

public class GetAllReviewByTargetIdQuery : IRequest<PagingResultDto<ReviewProductResponseDto>>
{
    public GetAllReviewQueryParams Params { get; set; }
}
