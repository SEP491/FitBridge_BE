using FitBridge_Application.Dtos.Accounts.HotResearch;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Specifications.Accounts.GetHotResearchAccount;
using MediatR;
using AutoMapper;
using FitBridge_Application.Dtos;

namespace FitBridge_Application.Features.Accounts.GetHotResearchAccount;

public class GetHotResearchAccountQueryHandler(IApplicationUserService _applicationUserService, IMapper _mapper) : IRequestHandler<GetHotResearchAccountQuery, PagingResultDto<HotResearchAccountDto>>
{
    public async Task<PagingResultDto<HotResearchAccountDto>> Handle(GetHotResearchAccountQuery request, CancellationToken cancellationToken)
    {
        var accounts = await _applicationUserService.GetAllUsersWithSpecAsync(new GetHotResearchAccountQuerySpec(request.Params));
        var accountsDto = new List<HotResearchAccountDto>();
        foreach (var account in accounts)
        {
            var accountDto = _mapper.Map<HotResearchAccountDto>(account);
            accountDto.UserRole = await _applicationUserService.GetUserRoleAsync(account);
            accountsDto.Add(accountDto);
        }
        var totalItems = await _applicationUserService.CountAsync(new GetHotResearchAccountQuerySpec(request.Params));
        return new PagingResultDto<HotResearchAccountDto>(totalItems, accountsDto);
    }
}
