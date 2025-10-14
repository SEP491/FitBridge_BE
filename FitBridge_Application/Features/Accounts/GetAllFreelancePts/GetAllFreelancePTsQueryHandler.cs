using System;
using AutoMapper;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Accounts.FreelancePts;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Specifications.Accounts.GetAllFreelancePts;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByFreelancePtId;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByFreelancePtIdCount;
using FitBridge_Domain.Entities.Gyms;
using MediatR;

namespace FitBridge_Application.Features.Accounts.GetAllFreelancePts;

public class GetAllFreelancePTsQueryHandler(IApplicationUserService _applicationUserService, IMapper _mapper, IUnitOfWork _unitOfWork) : IRequestHandler<GetAllFreelancePTsQuery, PagingResultDto<GetAllFreelancePTsResponseDto>>
{
    public async Task<PagingResultDto<GetAllFreelancePTsResponseDto>> Handle(GetAllFreelancePTsQuery request, CancellationToken cancellationToken)
    {
        var freelancePtsRaw = await _applicationUserService.GetUsersByRoleAsync(ProjectConstant.UserRoles.FreelancePT);
        var spec = new GetAllFreelancePtsSpec(request.Params, freelancePtsRaw.Select(x => x.Id).ToList());
        var freelancePtsEntity = await _applicationUserService.GetAllUsersWithSpecAsync(spec);
        var freelancePtDtos = new List<GetAllFreelancePTsResponseDto>();
        foreach (var freelancePt in freelancePtsEntity)
        {
            var freelancePtDto = _mapper.Map<GetAllFreelancePTsResponseDto>(freelancePt);
            var customerPurchasedSpec = new GetCustomerPurchasedByFreelancePtIdCountSpec(freelancePt.Id);
            var customerPurchaseds = await _unitOfWork.Repository<CustomerPurchased>().GetAllWithSpecificationAsync(customerPurchasedSpec);
            var totalPurchased = customerPurchaseds.Sum(x => x.OrderItems.Count);
            freelancePtDto.TotalPurchased = totalPurchased;
            freelancePtDtos.Add(freelancePtDto);
        }
        var freelancePtsCount = freelancePtsRaw.Count;
        return new PagingResultDto<GetAllFreelancePTsResponseDto>(freelancePtsCount, freelancePtDtos);
    }
}
