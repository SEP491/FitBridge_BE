using System;
using FitBridge_Application.Dtos.GymCourses.Response;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Domain.Entities.Gyms;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.GymCourses.Commands;

public class CreateGymCourseCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<CreateGymCourseCommand, CreateGymCourseResponse>
{
    public async Task<CreateGymCourseResponse> Handle(CreateGymCourseCommand request, CancellationToken cancellationToken)
    {
        var gymOwnerId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (gymOwnerId == null)
        {
            throw new Exception("Gym owner not found, please login to with gym owner account");
        }

        request.GymOwnerId = gymOwnerId.Value;
        var gymCourseRequest = _mapper.Map<GymCourse>(request);
        var gymCourse = _unitOfWork.Repository<GymCourse>().Insert(gymCourseRequest);
        await _unitOfWork.CommitAsync();
        var gymCourseResponse = _mapper.Map<CreateGymCourseResponse>(gymCourse);
        return gymCourseResponse;
    }

}
