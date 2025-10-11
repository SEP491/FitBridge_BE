using System;
using FitBridge_Application.Dtos.ActivitySets;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using FitBridge_Domain.Entities.Trainings;
using AutoMapper;
namespace FitBridge_Application.Features.ActivitySets.UpdateActivityProgress;

public class UpdateActivityProgressCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<UpdateActivityProgressCommand, List<ActivitySetResponseDto>>
{
    public async Task<List<ActivitySetResponseDto>> Handle(UpdateActivityProgressCommand request, CancellationToken cancellationToken)
    {
        return new List<ActivitySetResponseDto>();
    }
}
