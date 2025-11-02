using System;
using FitBridge_Application.Dtos.Gym;
using MediatR;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Interfaces.Services;
using AutoMapper;

namespace FitBridge_Application.Features.Accounts.GetGymPTByIdForAdmin;

public class GetGymPTByIdForAdminQueryHandler(IApplicationUserService _applicationUserService, IMapper _mapper) : IRequestHandler<GetGymPTByIdForAdminQuery, GetGymPtsDetailForAdminResponseDto>
{
    public async Task<GetGymPtsDetailForAdminResponseDto> Handle(GetGymPTByIdForAdminQuery request, CancellationToken cancellationToken)
    {
        var gymPT = await _applicationUserService.GetByIdAsync(request.Id, includes: new List<string> { "UserDetail", "GoalTrainings", "GymReviews", "GymOwner" });
        if (gymPT == null || !gymPT.IsEnabled)
        {
            throw new NotFoundException("Gym PT not found");
        }
        return _mapper.Map<GetGymPtsDetailForAdminResponseDto>(gymPT);
    }

}
