using System;
using FitBridge_Application.Dtos.Accounts;
using MediatR;

namespace FitBridge_Application.Features.Accounts.GetUserProfile;

public class GetProfileCommand : IRequest<GetUserProfileResponse>
{
    public Guid? AccountId { get; set; }
}
