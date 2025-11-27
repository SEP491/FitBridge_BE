using System;
using AutoMapper;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos.Accounts;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Specifications.Accounts;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Accounts.GetUserProfile;

public class GetProfileCommandHandler(IApplicationUserService applicationUserService, IMapper _mapper) : IRequestHandler<GetProfileCommand, GetUserProfileResponse>
{
    public async Task<GetUserProfileResponse> Handle(GetProfileCommand request, CancellationToken cancellationToken)
    {
        var account = await applicationUserService.GetUserWithSpecAsync(new GetAccountByIdSpecificationForUserProfile(request.AccountId ?? Guid.Empty));

        if (account == null)
        {
            throw new NotFoundException("Account not found");
        }

        var profileDto = _mapper.Map<GetUserProfileResponse>(account);
        return profileDto;
    }
}