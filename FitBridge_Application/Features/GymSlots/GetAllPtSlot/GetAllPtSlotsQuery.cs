using System;
using MediatR;
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Application.Specifications.GymSlots;
using FitBridge_Application.Dtos;

namespace FitBridge_Application.Features.GymSlots.GetAllPtSlot;

public class GetAllPtSlotsQuery : IRequest<PagingResultDto<GetPTSlot>>
{
    public GetAllPtSlotsParams Params { get; set; }
}
