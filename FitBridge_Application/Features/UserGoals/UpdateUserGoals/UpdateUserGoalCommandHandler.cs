using System;
using FitBridge_Application.Dtos.UserGoals;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using AutoMapper;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using FitBridge_Application.Interfaces.Services;

namespace FitBridge_Application.Features.UserGoals.UpdateUserGoals;

public class UpdateUserGoalCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor, IApplicationUserService _applicationUserService) : IRequestHandler<UpdateUserGoalCommand, UserGoalsDto>
{
    public async Task<UserGoalsDto> Handle(UpdateUserGoalCommand request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new NotFoundException("User not found");
        }
        var userEntity = await _applicationUserService.GetByIdAsync(userId.Value, includes: new List<string> { "UserDetail" }, true);
        if (userEntity == null)
        {
            throw new NotFoundException("User not found");
        }
        if(userEntity.UserDetail != null)
        {
            userEntity.UserDetail.Biceps = request.CurrentBiceps ?? userEntity.UserDetail.Biceps;
            userEntity.UserDetail.ForeArm = request.CurrentForeArm ?? userEntity.UserDetail.ForeArm;
            userEntity.UserDetail.Thigh = request.CurrentThigh ?? userEntity.UserDetail.Thigh;
            userEntity.UserDetail.Calf = request.CurrentCalf ?? userEntity.UserDetail.Calf;
            userEntity.UserDetail.Chest = request.CurrentChest ?? userEntity.UserDetail.Chest;
            userEntity.UserDetail.Waist = request.CurrentWaist ?? userEntity.UserDetail.Waist;
            userEntity.UserDetail.Hip = request.CurrentHip ?? userEntity.UserDetail.Hip;
            userEntity.UserDetail.Shoulder = request.CurrentShoulder ?? userEntity.UserDetail.Shoulder;
            userEntity.UserDetail.Height = request.CurrentHeight ?? userEntity.UserDetail.Height;
            userEntity.UserDetail.Weight = request.CurrentWeight ?? userEntity.UserDetail.Weight;
        }
        var customerPurchased = await _unitOfWork.Repository<CustomerPurchased>().GetByIdAsync(request.CustomerPurchasedId, false, new List<string> { "UserGoal" });
        if (customerPurchased == null)
        {
            throw new NotFoundException("Customer purchased not found");
        }
        var userGoal = customerPurchased.UserGoal;
        if (userGoal == null)
        {
            throw new NotFoundException("User goal not found");
        }
        userGoal.TargetBiceps = request.TargetBiceps ?? userGoal.TargetBiceps;
        userGoal.TargetForeArm = request.TargetForeArm ?? userGoal.TargetForeArm;
        userGoal.TargetThigh = request.TargetThigh ?? userGoal.TargetThigh;
        userGoal.TargetCalf = request.TargetCalf ?? userGoal.TargetCalf;
        userGoal.TargetChest = request.TargetChest ?? userGoal.TargetChest;
        userGoal.TargetWaist = request.TargetWaist ?? userGoal.TargetWaist;
        userGoal.TargetHip = request.TargetHip ?? userGoal.TargetHip;
        userGoal.TargetShoulder = request.TargetShoulder ?? userGoal.TargetShoulder;
        userGoal.TargetHeight = request.TargetHeight ?? userGoal.TargetHeight;
        userGoal.TargetWeight = request.TargetWeight ?? userGoal.TargetWeight;

        userGoal.StartBiceps = request.StartBiceps ?? userGoal.StartBiceps;
        userGoal.StartForeArm = request.StartForeArm ?? userGoal.StartForeArm;
        userGoal.StartThigh = request.StartThigh ?? userGoal.StartThigh;
        userGoal.StartCalf = request.StartCalf ?? userGoal.StartCalf;
        userGoal.StartChest = request.StartChest ?? userGoal.StartChest;
        userGoal.StartWaist = request.StartWaist ?? userGoal.StartWaist;
        userGoal.StartHip = request.StartHip ?? userGoal.StartHip;
        userGoal.StartShoulder = request.StartShoulder ?? userGoal.StartShoulder;
        userGoal.StartHeight = request.StartHeight ?? userGoal.StartHeight;
        userGoal.StartWeight = request.StartWeight ?? userGoal.StartWeight;

        userGoal.FinalBiceps = request.CurrentBiceps ?? userGoal.FinalBiceps;
        userGoal.FinalForeArm = request.CurrentForeArm ?? userGoal.FinalForeArm;
        userGoal.FinalThigh = request.CurrentThigh ?? userGoal.FinalThigh;
        userGoal.FinalCalf = request.CurrentCalf ?? userGoal.FinalCalf;
        userGoal.FinalChest = request.CurrentChest ?? userGoal.FinalChest;
        userGoal.FinalWaist = request.CurrentWaist ?? userGoal.FinalWaist;
        userGoal.FinalHip = request.CurrentHip ?? userGoal.FinalHip;
        userGoal.FinalShoulder = request.CurrentShoulder ?? userGoal.FinalShoulder;
        userGoal.FinalHeight = request.CurrentHeight ?? userGoal.FinalHeight;
        userGoal.FinalWeight = request.CurrentWeight ?? userGoal.FinalWeight;
        userGoal.ImageUrl = request.ImageUrl ?? userGoal.ImageUrl;
        userGoal.FinalImageUrl = request.FinalImageUrl ?? userGoal.FinalImageUrl;
        
        await _unitOfWork.CommitAsync();
        return _mapper.Map<UserGoal, UserGoalsDto>(userGoal);
    }

}
