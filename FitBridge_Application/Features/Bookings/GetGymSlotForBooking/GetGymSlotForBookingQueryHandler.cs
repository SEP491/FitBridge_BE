using System;
using AutoMapper;
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Services;
using MediatR;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Application.Specifications.Bookings.GetGymSlotForBooking;
using FitBridge_Application.Specifications.GymSlotPts.GetPTGymSlotForBooking;
using FitBridge_Application.Dtos;

namespace FitBridge_Application.Features.Bookings.GetGymSlotForBooking;

public class GetGymSlotForBookingQueryHandler(
  IUnitOfWork _unitOfWork,
  IMapper _mapper,
  IUserUtil _userUtil,
 IHttpContextAccessor _httpContextAccessor) : IRequestHandler<GetGymSlotForBookingQuery, PagingResultDto<GetPtGymSlotForBookingResponse>>
{
    public async Task<PagingResultDto<GetPtGymSlotForBookingResponse>> Handle(GetGymSlotForBookingQuery request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new NotFoundException("UserId");
        }

        var spec = new GetPTGymSlotForBookingSpecification(request.Params);
        var gymSlot = await _unitOfWork.Repository<PTGymSlot>().GetAllWithSpecificationProjectedAsync<GetPtGymSlotForBookingResponse>(spec, _mapper.ConfigurationProvider);
        if (gymSlot == null)
        {
            throw new NotFoundException("GymSlot");
        }
        return new PagingResultDto<GetPtGymSlotForBookingResponse>(gymSlot.Count, gymSlot);
    }
}