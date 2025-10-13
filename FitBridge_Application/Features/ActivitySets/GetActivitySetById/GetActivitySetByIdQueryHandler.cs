using System;
using AutoMapper;
using FitBridge_Application.Dtos.ActivitySets;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Features.ActivitySets.GetActivitySetById;

public class GetActivitySetByIdQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetActivitySetByIdQuery, ActivitySetResponseDto>
{
    public async Task<ActivitySetResponseDto> Handle(GetActivitySetByIdQuery request, CancellationToken cancellationToken)
    {
        var activitySet = await _unitOfWork.Repository<ActivitySet>().GetByIdAsync(request.Id);
        if (activitySet == null)
        {
            throw new NotFoundException("Activity set not found");
        }
        return _mapper.Map<ActivitySetResponseDto>(activitySet);
    }

}
