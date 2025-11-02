using System;
using FitBridge_Application.Dtos.Gym;
using MediatR;

namespace FitBridge_Application.Features.Accounts.GetGymPTByIdForAdmin;

public class GetGymPTByIdForAdminQuery : IRequest<GetGymPtsDetailForAdminResponseDto>
{
    public Guid Id { get; set; }
}
