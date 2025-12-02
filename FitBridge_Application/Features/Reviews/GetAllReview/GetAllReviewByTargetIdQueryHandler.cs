using System;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Reviews;
using FitBridge_Application.Specifications.Reviews.GetAllReview;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.Ecommerce;
using AutoMapper;

namespace FitBridge_Application.Features.Reviews.GetAllReview;

public class GetAllReviewByTargetIdQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetAllReviewByTargetIdQuery, PagingResultDto<ReviewProductResponseDto>>
{
    public async Task<PagingResultDto<ReviewProductResponseDto>> Handle(GetAllReviewByTargetIdQuery request, CancellationToken cancellationToken)
    {
        var (targetId, spec) = await DetermineTargetId(request);
        var reviews = await _unitOfWork.Repository<Review>().GetAllWithSpecificationAsync(spec);
        var totalItems = await _unitOfWork.Repository<Review>().CountAsync(spec);
        var dtos = _mapper.Map<List<ReviewProductResponseDto>>(reviews);
        return new PagingResultDto<ReviewProductResponseDto>(totalItems, dtos);
    }

    public async Task<(Guid, GetAllReviewByTargetIdSpec)> DetermineTargetId(GetAllReviewByTargetIdQuery request)
    {
        var gymCourseId = request.Params.GymCourseId;
        var freelancePtCourseId = request.Params.FreelancePtCourseId;
        var productId = request.Params.ProductId;
        if(freelancePtCourseId != null)
        {
            var freelancePTPackage = await _unitOfWork.Repository<FreelancePTPackage>().GetByIdAsync(freelancePtCourseId.Value);
            if(freelancePTPackage == null)
            {
                throw new NotFoundException("Freelance pt package not found");
            }
            return (freelancePTPackage.PtId, new GetAllReviewByTargetIdSpec(request.Params, FreelancePtId: freelancePTPackage.PtId));
        }
        if(productId != null)
        {
            return (productId.Value, new GetAllReviewByTargetIdSpec(request.Params, ProductId: productId.Value));
        }
        if(gymCourseId != null)
        {
            var gymCourse = await _unitOfWork.Repository<GymCourse>().GetByIdAsync(gymCourseId.Value);
            if(gymCourse == null)
            {
                throw new NotFoundException("Gym course not found");
            }
            return (gymCourse.GymOwnerId, new GetAllReviewByTargetIdSpec(request.Params, GymId: gymCourse.GymOwnerId));
        }
        throw new NotFoundException("Target not found");
    }
}
