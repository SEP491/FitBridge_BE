using System;
using FitBridge_Application.Dtos.Accounts.FreelancePts;
using MediatR;

namespace FitBridge_Application.Features.Accounts.GetFreelancePtById;

public class GetFreelancePTByIdQuery : IRequest<GetFreelancePtByIdResponseDto>
{
    public Guid Id { get; set; }
}
