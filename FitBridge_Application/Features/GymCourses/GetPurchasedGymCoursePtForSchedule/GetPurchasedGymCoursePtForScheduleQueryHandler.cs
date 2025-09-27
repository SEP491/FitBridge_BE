using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.GymCourses;
using FitBridge_Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetAvailableGymCoursePtInCustomerPurchased;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Features.GymCourses.GetPurchasedGymCoursePtForSchedule;

public class GetPurchasedGymCoursePtForScheduleQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<GetPurchasedGymCoursePtForScheduleQuery, PagingResultDto<GymCoursesPtResponse>>
{
    public async Task<PagingResultDto<GymCoursesPtResponse>> Handle(GetPurchasedGymCoursePtForScheduleQuery request, CancellationToken cancellationToken)
    {
        var customerId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (customerId == null)
        {
            throw new NotFoundException("Customer id not found");
        }
        var spec = new GetAvailableGymCoursePtInCustomerPurchasedSpec(request.Params, customerId.Value);
        var results = await _unitOfWork.Repository<CustomerPurchased>().GetAllWithSpecificationProjectedAsync<GymCoursesPtResponse>(spec, _mapper.ConfigurationProvider);
        
        var totalItems = await _unitOfWork.Repository<CustomerPurchased>().CountAsync(spec);
        return new PagingResultDto<GymCoursesPtResponse>(totalItems, results);
    }
}
