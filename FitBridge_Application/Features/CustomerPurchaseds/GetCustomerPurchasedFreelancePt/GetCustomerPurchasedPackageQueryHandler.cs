using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.CustomerPurchaseds;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByCustomerId;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using MediatR;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchasedFreelancePt;

public class GetCustomerPurchasedPackageQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<GetCustomerPurchasedPackage, CustomerPurchasedForMyPackageDto>
{
    public async Task<CustomerPurchasedForMyPackageDto> Handle(GetCustomerPurchasedPackage request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new NotFoundException("User not found");
        }
        var result = new CustomerPurchasedForMyPackageDto();
        result.freelancePtPackages = await AggregateFreelancePtCourse(request.Params, userId.Value);
        result.gymCourses = await AggregateGymCourse(request.Params, userId.Value);

        return result;
    }

    public async Task<PagingResultDto<CustomerPurchasedFreelancePtResponseDto>> AggregateFreelancePtCourse(GetCustomerPurchasedParams request, Guid userId)
    {
        var FreelancePtspec = new GetCustomerPurchasedByCustomerIdSpec(userId, request, false);

        var customerPurchasedsForFreelancePt = await _unitOfWork.Repository<CustomerPurchased>().GetAllWithSpecificationProjectedAsync<CustomerPurchasedFreelancePtResponseDto>(FreelancePtspec, _mapper.ConfigurationProvider);

        var totalItemsForFreelancePt = await _unitOfWork.Repository<CustomerPurchased>().CountAsync(FreelancePtspec);

        return new PagingResultDto<CustomerPurchasedFreelancePtResponseDto>(totalItemsForFreelancePt, customerPurchasedsForFreelancePt);
    }

    public async Task<PagingResultDto<CustomerPurchasedResponseDto>> AggregateGymCourse(GetCustomerPurchasedParams request, Guid userId)
    {
        var GymCoursespec = new GetCustomerPurchasedByCustomerIdSpec(userId, request, true);

        var customerPurchasedsForGymCourse = await _unitOfWork.Repository<CustomerPurchased>().GetAllWithSpecificationProjectedAsync<CustomerPurchasedResponseDto>(GymCoursespec, _mapper.ConfigurationProvider);

        var totalItemsForGymCourse = await _unitOfWork.Repository<CustomerPurchased>().CountAsync(GymCoursespec);

        return new PagingResultDto<CustomerPurchasedResponseDto>(totalItemsForGymCourse, customerPurchasedsForGymCourse);
    }
}