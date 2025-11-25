using System;
using MediatR;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Application.Dtos.Reviews;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Services;
using FitBridge_Domain.Entities.Orders;
using AutoMapper;

namespace FitBridge_Application.Features.Reviews.CreateReview;

public class CreateReviewCommandHandler(IUnitOfWork _unitOfWork, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor, IUploadService _uploadService, SystemConfigurationService _systemConfigurationService, IScheduleJobServices _scheduleJobServices, IMapper _mapper) : IRequestHandler<CreateReviewCommand, ReviewProductResponseDto>
{
    public async Task<ReviewProductResponseDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {

        var maximumReviewImages = (int)await _systemConfigurationService.GetSystemConfigurationAutoConvertDataTypeAsync(ProjectConstant.SystemConfigurationKeys.MaximumReviewImages);
        if (request.Images != null && request.Images.Count > maximumReviewImages)
        {
            throw new BusinessException($"Maximum review images is {maximumReviewImages}");
        }
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new NotFoundException("User not found");
        }
        var orderItem = await _unitOfWork.Repository<OrderItem>().GetByIdAsync(request.OrderItemId.Value, includes: new List<string> { "GymCourse", "FreelancePTPackage", "ProductDetail", "Order.Account" });
        if (orderItem == null)
        {
            throw new NotFoundException("Order item not found");
        }
        if(orderItem.IsFeedback)
            {
            throw new FeedbackExistException($"Order item {orderItem.Id} already has a review");
        }
        List<string> imageUrls = new List<string>();
        if (request.Images != null)
        {
            foreach (var image in request.Images)
            {
                var imageUrl = await _uploadService.UploadFileAsync(image);
                imageUrls.Add(imageUrl);
            }
        }
        var review = new Review
        {
            Rating = request.Rating,
            Content = request.Content,
            IsEdited = false,
            UserId = userId.Value,
            ImageUrls = imageUrls,
            GymId = orderItem.GymCourse != null ? orderItem.GymCourse.GymOwnerId : null,
            FreelancePtId = orderItem.FreelancePTPackage != null ? orderItem.FreelancePTPackage.PtId : null,
            ProductDetailId = orderItem.ProductDetail != null ? orderItem.ProductDetail.Id : null,
        };
        orderItem.IsFeedback = true;
        await _scheduleJobServices.CancelScheduleJob($"AutoMarkAsFeedback_{orderItem.Id}", "AutoMarkAsFeedback");
        _unitOfWork.Repository<OrderItem>().Update(orderItem);
        _unitOfWork.Repository<Review>().Insert(review);
        await _unitOfWork.CommitAsync();
        var reviewDto = _mapper.Map<ReviewProductResponseDto>(review);
        reviewDto.UserAvatarUrl = orderItem.Order.Account.AvatarUrl;
        reviewDto.UserName = orderItem.Order.Account.FullName;
        return reviewDto;
    }
}
