using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Bookings;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Bookings.GetBookingRequests;
using MediatR;
using AutoMapper;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Commons.Constants;

namespace FitBridge_Application.Features.Bookings.GetBookingRequest;

public class GetBookingRequestQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<GetBookingRequestQuery, PagingResultDto<GetBookingRequestResponse>>
{
    public async Task<PagingResultDto<GetBookingRequestResponse>> Handle(GetBookingRequestQuery request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if(userId == null)
        {
            throw new NotFoundException("User not found");
        }
        var role = _userUtil.GetUserRole(_httpContextAccessor.HttpContext);
        var spec = new GetBookingRequestPtSpecification(request.Params, null, userId);
        if (role == ProjectConstant.UserRoles.Customer)
        {
            spec = new GetBookingRequestPtSpecification(request.Params, userId, null);
        }
        var bookingRequests = await _unitOfWork.Repository<BookingRequest>().GetAllWithSpecificationProjectedAsync<GetBookingRequestResponse>(spec, _mapper.ConfigurationProvider);
        var totalItems = await _unitOfWork.Repository<BookingRequest>().CountAsync(spec);
        return new PagingResultDto<GetBookingRequestResponse>
        (
            totalItems,
            bookingRequests
        );
    }

}
