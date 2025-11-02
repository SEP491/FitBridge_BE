using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Specifications.Accounts.GetAllGymPtsForAdmin;
using MediatR;

namespace FitBridge_Application.Features.Accounts.GetAllGymPtsForAdmin;

public class GetAllGymPtsForAdminQuery : IRequest<PagingResultDto<GetAllGymPtsForAdminResponseDto>>
{
    public GetAllGymPtsForAdminParams Params { get; set; }
}
