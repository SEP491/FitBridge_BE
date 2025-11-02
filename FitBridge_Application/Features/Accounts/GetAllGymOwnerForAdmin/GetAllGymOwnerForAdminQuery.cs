using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Specifications.Accounts.GetAllGymOwnerForAdmin;
using MediatR;

namespace FitBridge_Application.Features.Accounts.GetAllGymOwnerForAdmin;

public class GetAllGymOwnerForAdminQuery : IRequest<PagingResultDto<GetAllGymOwnerForAdminDto>>
{
    public GetAllGymOwnerForAdminParams Params { get; set; }
}
