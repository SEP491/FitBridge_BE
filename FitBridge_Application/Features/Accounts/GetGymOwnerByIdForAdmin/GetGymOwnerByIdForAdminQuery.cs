using System;
using FitBridge_Application.Dtos.Gym;
using MediatR;

namespace FitBridge_Application.Features.Accounts.GetGymOwnerByIdForAdmin;

public class GetGymOwnerByIdForAdminQuery : IRequest<GetGymOwnerDetailForAdminDto>
{
    public Guid Id { get; set; }
}
