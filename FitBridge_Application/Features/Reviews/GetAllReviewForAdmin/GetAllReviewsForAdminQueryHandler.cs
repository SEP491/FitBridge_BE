using System;
using FitBridge_Application.Dtos.Reviews;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Reviews.GetAllReviewForAdmin;
using MediatR;
using AutoMapper;
using FitBridge_Domain.Entities.MessageAndReview;

namespace FitBridge_Application.Features.Reviews.GetAllReviewForAdmin;

public class GetAllReviewsForAdminQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetAllReviewsForAdminQuery, List<ReviewProductResponseDto>>
{
    public async Task<List<ReviewProductResponseDto>> Handle(GetAllReviewsForAdminQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetAllReviewsForAdminSpec(request.Params);
        var reviews = await _unitOfWork.Repository<Review>().GetAllWithSpecificationProjectedAsync<ReviewProductResponseDto>(spec, _mapper.ConfigurationProvider);
        return reviews.ToList();
    }
}
