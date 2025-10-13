using System;
using AutoMapper;
using FitBridge_Application.Dtos.SessionActivities;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Features.SessionActivities.GetSessionActivityById;

public class GetSessionActivityByIdQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetSessionActivityByIdQuery, SessionActivityResponseDto>
{
    public async Task<SessionActivityResponseDto> Handle(GetSessionActivityByIdQuery request, CancellationToken cancellationToken)
    {
        var sessionActivity = await _unitOfWork.Repository<SessionActivity>().GetByIdAsync(request.Id, includes: new List<string> { nameof(SessionActivity.ActivitySets) });
        if (sessionActivity == null)
        {
            throw new NotFoundException("Session activity not found");
        }
        return _mapper.Map<SessionActivityResponseDto>(sessionActivity);
    }

}
