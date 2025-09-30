using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using FitBridge_Application.Specifications.GymSlots;
using FitBridge_Domain.Entities.Gyms;
using AutoMapper;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Features.GymSlots.GetAllGymSlot;

public class GetGymSlotsQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<GetGymSlotsQuery, PagingResultDto<SlotResponseDto>>
{
    public async Task<PagingResultDto<SlotResponseDto>> Handle(GetGymSlotsQuery request, CancellationToken cancellationToken)
    {
        var gymId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (gymId == null)
        {
            throw new NotFoundException("Gym not found");
        }
        var spec = new GetAllGymSlotsSpec(request.Params, gymId.Value);
        var result = await _unitOfWork.Repository<GymSlot>().GetAllWithSpecificationProjectedAsync<SlotResponseDto>(spec, _mapper.ConfigurationProvider);
        var totalItems = await _unitOfWork.Repository<GymSlot>().CountAsync(spec);
        return new PagingResultDto<SlotResponseDto>(totalItems, result);
    }

}
