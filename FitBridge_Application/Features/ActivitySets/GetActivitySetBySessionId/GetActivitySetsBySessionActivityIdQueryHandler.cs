using System;
using FitBridge_Application.Dtos.ActivitySets;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using FitBridge_Domain.Entities.Trainings;
using AutoMapper;
using FitBridge_Application.Specifications.ActivitySets.GetActivitySetsBySessionActivityId;

namespace FitBridge_Application.Features.ActivitySets.GetActivitySetBySessionId;

public class GetActivitySetsBySessionActivityIdQueryHandler(IUnitOfWork unitOfWork, IMapper _mapper) : IRequestHandler<GetActivitySetsBySessionActivityIdQuery, List<ActivitySetResponseDto>>
{   
    public async Task<List<ActivitySetResponseDto>> Handle(GetActivitySetsBySessionActivityIdQuery request, CancellationToken cancellationToken)
    {
        var activitySets = await unitOfWork.Repository<ActivitySet>().GetAllWithSpecificationProjectedAsync<ActivitySetResponseDto>(new GetActivitySetsBySessionActivityIdSpecification(request.SessionActivityId), _mapper.ConfigurationProvider);
        return activitySets.ToList();
    }

}
