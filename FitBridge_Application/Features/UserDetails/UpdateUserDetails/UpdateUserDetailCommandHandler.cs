using System;
using FitBridge_Application.Dtos.Accounts.UserDetails;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using AutoMapper;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Features.UserDetails.UpdateUserDetails;

public class UpdateUserDetailCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<UpdateUserDetailCommand, UserDetailDto>
{
    public async Task<UserDetailDto> Handle(UpdateUserDetailCommand request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext)
            ?? throw new NotFoundException(nameof(ApplicationUser));
        var userDetail = await _unitOfWork.Repository<UserDetail>().GetByIdAsync(userId);
        if (userDetail == null)
        {
            throw new NotFoundException(nameof(UserDetail));
        }
        userDetail.Biceps = request.Biceps ?? userDetail.Biceps;
        userDetail.ForeArm = request.ForeArm ?? userDetail.ForeArm;
        userDetail.Thigh = request.Thigh ?? userDetail.Thigh;
        userDetail.Calf = request.Calf ?? userDetail.Calf;
        userDetail.Chest = request.Chest ?? userDetail.Chest;
        userDetail.Waist = request.Waist ?? userDetail.Waist;
        userDetail.Hip = request.Hip ?? userDetail.Hip;
        userDetail.Shoulder = request.Shoulder ?? userDetail.Shoulder;
        userDetail.Height = request.Height ?? userDetail.Height;
        userDetail.Weight = request.Weight ?? userDetail.Weight;
        userDetail.Bio = request.Bio ?? userDetail.Bio;
        userDetail.Experience = request.Experience ?? userDetail.Experience;
        userDetail.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<UserDetail>().Update(userDetail);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<UserDetailDto>(userDetail);
    }
}
