using System;
using FitBridge_Application.Dtos.Reviews;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Services;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using FitBridge_Application.Commons.Constants;
namespace FitBridge_Application.Features.Reviews.UpdateReview;

public class UpdateReviewCommandHandler(IUnitOfWork _unitOfWork, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor, IUploadService _uploadService, SystemConfigurationService _systemConfigurationService, IScheduleJobServices _scheduleJobServices, IMapper _mapper) : IRequestHandler<UpdateReviewCommand, ReviewProductResponseDto>
{
    public async Task<ReviewProductResponseDto> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var maximumReviewImages = (int)await _systemConfigurationService.GetSystemConfigurationAutoConvertDataTypeAsync(ProjectConstant.SystemConfigurationKeys.MaximumReviewImages);
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new NotFoundException("User not found");
        }
        var review = await _unitOfWork.Repository<Review>().GetByIdAsync(request.ReviewId, includes: new List<string> { "User" });
        if (review == null)
        {
            throw new NotFoundException($"Review {request.ReviewId} not found");
        }
        if(review.IsEdited)
        {
            throw new BusinessException("Review already edited");
        }
        // if (review.UserId != userId.Value)
        // {
        //     throw new BusinessException("You are not allowed to update this review");
        // }
        if (request.Rating != null)
        {
            review.Rating = request.Rating.Value;
        }
        if (request.Images != null)
        {
            if (request.Images.Count > maximumReviewImages)
            {
                throw new BusinessException($"Maximum review images is {maximumReviewImages}");
            }
            var imageUrls = new List<string>();
            foreach (var image in request.Images)
            {
                var imageUrl = await _uploadService.UploadFileAsync(image);
                imageUrls.Add(imageUrl);
            }
            review.ImageUrls = imageUrls;
        }
        review.Content = request.Content ?? review.Content;
        review.Rating = request.Rating ?? review.Rating;
        review.IsEdited = true;
        review.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<Review>().Update(review);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<ReviewProductResponseDto>(review);
    }

}
