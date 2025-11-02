using System;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Interfaces.Services;
using MediatR;
using AutoMapper;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Features.Accounts.GetGymOwnerByIdForAdmin;

public class GetGymOwnerByIdForAdminQueryHandler(IApplicationUserService _applicationUserService, IMapper _mapper) : IRequestHandler<GetGymOwnerByIdForAdminQuery, GetGymOwnerDetailForAdminDto>
{
    public async Task<GetGymOwnerDetailForAdminDto> Handle(GetGymOwnerByIdForAdminQuery request, CancellationToken cancellationToken)
    {
        var userEntity = await _applicationUserService.GetByIdAsync(request.Id);
        if (userEntity == null)
        {
            throw new NotFoundException("Gym owner not found");
        }
        var user = _mapper.Map<GetGymOwnerDetailForAdminDto>(userEntity);
        return user;
    }
}
