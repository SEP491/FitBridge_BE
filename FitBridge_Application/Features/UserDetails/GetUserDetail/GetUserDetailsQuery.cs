using System;
using FitBridge_Application.Dtos.Accounts.UserDetails;
using MediatR;

namespace FitBridge_Application.Features.UserDetails.GetUserDetail;

public class GetUserDetailsQuery : IRequest<UserDetailDto>
{
    public Guid? AccountId { get; set; }
}
