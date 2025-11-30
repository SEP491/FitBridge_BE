using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Reviews;
using FitBridge_Application.Specifications.Reviews.GetAllReviewForCustomer;
using MediatR;

namespace FitBridge_Application.Features.Reviews.GetCustomerReviews;

public class GetCustomerReviewQuery(GetCustomerReviewParams parameters) : IRequest<PagingResultDto<UserReviewResponseDto>>
{
    public GetCustomerReviewParams Params { get; set; } = parameters;
}
