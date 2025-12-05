using System;
using AutoMapper;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Entities.ServicePackages;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Subscriptions.UpdateSubscriptionPlan;

public class UpdateSubscriptionPlanCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IUploadService _uploadService) : IRequestHandler<UpdateSubscriptionPlanCommand, bool>  {
    public async Task<bool> Handle(UpdateSubscriptionPlanCommand request, CancellationToken cancellationToken)
    {
        var subscriptionPlan = await _unitOfWork.Repository<SubscriptionPlansInformation>().GetByIdAsync(request.Id);
        if (subscriptionPlan == null)
        {
            throw new NotFoundException("Subscription plan not found");
        }
        if (request.Name != null)
        {
            subscriptionPlan.PlanName = request.Name;
        }
        if (request.Charge != null)
        {
            subscriptionPlan.PlanCharge = request.Charge.Value;
        }
        if (request.Duration != null)
        {
            subscriptionPlan.Duration = request.Duration.Value;
        }
        if (request.Description != null)
        {
            subscriptionPlan.Description = request.Description;
        }
        if (request.ImageUrl != null)
        {
            if (subscriptionPlan.ImageUrl != null)
            {
                await _uploadService.DeleteFileAsync(subscriptionPlan.ImageUrl);
            }
            var imageUrl = await _uploadService.UploadFileAsync(request.ImageUrl);
            subscriptionPlan.ImageUrl = imageUrl;
        }
        _unitOfWork.Repository<SubscriptionPlansInformation>().Update(subscriptionPlan);
        await _unitOfWork.CommitAsync();
        return true;
    }
}
