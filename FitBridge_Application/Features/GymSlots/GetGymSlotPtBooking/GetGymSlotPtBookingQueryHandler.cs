using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.GymSlotPts.GetGymSlotPtBooking;
using FitBridge_Domain.Entities.Gyms;
using MediatR;
using AutoMapper;

namespace FitBridge_Application.Features.GymSlots.GetGymSlotPtBooking;

public class GetGymSlotPtBookingQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetGymSlotPtBookingQuery, PagingResultDto<GymSlotPtBookingDto>>        
{
    public async Task<PagingResultDto<GymSlotPtBookingDto>> Handle(GetGymSlotPtBookingQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetGymSlotPtBookingSpec(request.Params);
        var ptGymSlots = await _unitOfWork.Repository<PTGymSlot>().GetAllWithSpecificationProjectedAsync<GymSlotPtBookingDto>(spec, _mapper.ConfigurationProvider);
        var totalItems = await _unitOfWork.Repository<PTGymSlot>().CountAsync(spec);
        return new PagingResultDto<GymSlotPtBookingDto>(totalItems, ptGymSlots);
    }
}
