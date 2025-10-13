using System;
using FitBridge_Application.Dtos.Accounts.FreelancePts;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Exceptions;
using AutoMapper;
using MediatR;

namespace FitBridge_Application.Features.Accounts.GetFreelancePtById;

public class GetFreelancePTByIdQueryHandler(IApplicationUserService _applicationUserService, IMapper _mapper) : IRequestHandler<GetFreelancePTByIdQuery, GetFreelancePtByIdResponseDto>
{
    public async Task<GetFreelancePtByIdResponseDto> Handle(GetFreelancePTByIdQuery request, CancellationToken cancellationToken)
    {
        var freelancePt = await _applicationUserService.GetByIdAsync(request.Id, includes: new List<string>
        {
            "UserDetail",
            "PTFreelancePackages"
        });
        if (freelancePt == null)
        {
            throw new NotFoundException("Freelance PT not found");
        }
        return _mapper.Map<GetFreelancePtByIdResponseDto>(freelancePt);
    }
}
