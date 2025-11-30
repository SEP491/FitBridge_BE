using System;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Dtos.Reviews;
using FitBridge_Application.Specifications.Reviews.GetAllReviewForCustomer;
using AutoMapper;
using MediatR;
using FitBridge_Application.Dtos;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Application.Dtos.GymCourses;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Dtos.ProductDetails;
using FitBridge_Application.Dtos.FreelancePTPackages;

namespace FitBridge_Application.Features.Reviews.GetCustomerReviews;

public class GetCustomerReviewQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetCustomerReviewQuery, PagingResultDto<UserReviewResponseDto>>
{
    public async Task<PagingResultDto<UserReviewResponseDto>> Handle(GetCustomerReviewQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _unitOfWork.Repository<Review>().GetAllWithSpecificationAsync(new GetCustomerReviewSpec(request.Params), false);
        var reviewDtos = new List<UserReviewResponseDto>();
        foreach (var review in reviews)
        {
            var reviewDto = _mapper.Map<UserReviewResponseDto>(review);
            if (review.GymId != null)
            {
                reviewDto.GymBrief = new GymReviewBriefDto();
                reviewDto.GymBrief.Id = review.GymId.Value;
                reviewDto.GymBrief.GymName = review.Gym.GymName;
            }
            else if (review.FreelancePtId != null)
            {
                reviewDto.FreelancePtBrief = new FreelancePtReviewBriefDto();
                reviewDto.FreelancePtBrief.Id = review.FreelancePtId.Value;
                reviewDto.FreelancePtBrief.FullName = review.FreelancePt.FullName;
                reviewDto.FreelancePtBrief.ImageUrl = review.FreelancePt.AvatarUrl;
            }
            reviewDtos.Add(reviewDto);
        }
        var totalItems = await _unitOfWork.Repository<Review>().CountAsync(new GetCustomerReviewSpec(request.Params));
        return new PagingResultDto<UserReviewResponseDto>(totalItems, reviewDtos);
    }
}
