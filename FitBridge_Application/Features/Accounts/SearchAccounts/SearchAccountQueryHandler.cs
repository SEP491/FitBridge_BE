using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Accounts.Search;
using FitBridge_Application.Specifications.Accounts.GetAccountForSearching;
using FitBridge_Application.Interfaces.Services;
using MediatR;
using FitBridge_Application.Interfaces.Repositories;
using AutoMapper;
using FitBridge_Domain.Enums.ApplicationUser;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos.Accounts.FreelancePts;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByFreelancePtIdCount;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Dtos.Gym;

namespace FitBridge_Application.Features.Accounts.SearchAccounts;

public class SearchAccountQueryHandler(IApplicationUserService _applicationUserService, IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<SearchAccountQuery, GetAccountForSearchingResponseDto>
{
    public async Task<GetAccountForSearchingResponseDto> Handle(SearchAccountQuery request, CancellationToken cancellationToken)
    {
        var accountsResponseDto = new GetAccountForSearchingResponseDto();
        accountsResponseDto.FreelancePts = new PagingResultDto<GetAllFreelancePTsResponseDto>(0, new List<GetAllFreelancePTsResponseDto>());
        accountsResponseDto.Gyms = new PagingResultDto<GetAllGymsForSearchDto>(0, new List<GetAllGymsForSearchDto>());
        if (request.Params.SearchType == SearchType.FreelancePT || request.Params.SearchType == SearchType.FreelancePTAndGym)
        {
            await SearchFreelancePt(request, accountsResponseDto);
        }
        if (request.Params.SearchType == SearchType.Gym || request.Params.SearchType == SearchType.FreelancePTAndGym)
        {
            await SearchGym(request, accountsResponseDto);
        }
        return accountsResponseDto;

    }

    public async Task SearchFreelancePt(SearchAccountQuery request, GetAccountForSearchingResponseDto accountsResponseDto)
    {
        var freelancePtEntities = await _applicationUserService.GetUsersByRoleAsync(ProjectConstant.UserRoles.FreelancePT);
        var freelancePtIds = freelancePtEntities.Select(x => x.Id).ToList();
        var spec = new GetAccountForSearchingSpec(request.Params, freelancePtIds);
        var freelancePtAccounts = await _applicationUserService.GetAllUsersWithSpecAsync(spec);
        await AggregateFreelancePtResponseDto(freelancePtAccounts, request, accountsResponseDto);
    }

    public async Task AggregateFreelancePtResponseDto(IReadOnlyList<ApplicationUser> accounts, SearchAccountQuery request, GetAccountForSearchingResponseDto accountsResponseDto)
    {
        if (request.Params.SearchType == SearchType.Gym)
        {
            return;
        }
        var FreelancePtResponseDtos = new List<GetAllFreelancePTsResponseDto>();
        foreach (var account in accounts)
        {
            var freelancePtDto = _mapper.Map<GetAllFreelancePTsResponseDto>(account);
            var customerPurchasedSpec = new GetCustomerPurchasedByFreelancePtIdCountSpec(account.Id);
            var customerPurchaseds = await _unitOfWork.Repository<CustomerPurchased>().GetAllWithSpecificationAsync(customerPurchasedSpec);
            var totalPurchased = customerPurchaseds.Sum(x => x.OrderItems.Count);
            freelancePtDto.TotalPurchased = totalPurchased;
            FreelancePtResponseDtos.Add(freelancePtDto);
        }
        
        accountsResponseDto.FreelancePts = new PagingResultDto<GetAllFreelancePTsResponseDto>(FreelancePtResponseDtos.Count, FreelancePtResponseDtos);
    }

    public async Task SearchGym(SearchAccountQuery request, GetAccountForSearchingResponseDto accountsResponseDto)
    {
        var gymEntities = await _applicationUserService.GetUsersByRoleAsync(ProjectConstant.UserRoles.GymOwner);
        var gymIds = gymEntities.Select(x => x.Id).ToList();
        var spec = new GetAccountForSearchingSpec(request.Params, gymIds);
        var gymAccounts = await _applicationUserService.GetAllUsersWithSpecAsync(spec);
        await AggregateGymResponseDto(gymAccounts, request, accountsResponseDto);
    }

    public async Task AggregateGymResponseDto(IReadOnlyList<ApplicationUser> accounts, SearchAccountQuery request, GetAccountForSearchingResponseDto accountsResponseDto)
    {
        if (request.Params.SearchType == SearchType.FreelancePT)
        {
            return;
        }
        var GymResponseDtos = new List<GetAllGymsForSearchDto>();
        foreach (var account in accounts)
        {
            var gymDto = _mapper.Map<GetAllGymsForSearchDto>(account);
            GymResponseDtos.Add(gymDto);
        }
        accountsResponseDto.Gyms = new PagingResultDto<GetAllGymsForSearchDto>(GymResponseDtos.Count, GymResponseDtos);
    }
}
