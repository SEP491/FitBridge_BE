using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Bookings;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Bookings.GetFreelancePtSchedule;
using MediatR;
using AutoMapper;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Application.Specifications;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Features.Bookings.GetFreelancePtSchedule;

public class GetFreelancePtScheduleQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<GetFreelancePtScheduleQuery, PagingResultDto<GetFreelancePtScheduleResponse>>
{
    public async Task<PagingResultDto<GetFreelancePtScheduleResponse>> Handle(GetFreelancePtScheduleQuery request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new NotFoundException("Pt not found");
        }
        var spec = new GetFreelancePtScheduleSpec(request.Params, userId.Value);
        var bookings = await _unitOfWork.Repository<Booking>().GetAllWithSpecificationProjectedAsync<GetFreelancePtScheduleResponse>(spec, _mapper.ConfigurationProvider);
        var totalItems = await _unitOfWork.Repository<Booking>().CountAsync(spec);
        return new PagingResultDto<GetFreelancePtScheduleResponse>(totalItems, bookings);
    }

}
