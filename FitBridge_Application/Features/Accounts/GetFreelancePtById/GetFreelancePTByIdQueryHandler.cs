using System;
using FitBridge_Application.Dtos.Accounts.FreelancePts;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Exceptions;
using AutoMapper;
using MediatR;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByFreelancePtIdCount;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByPackageId;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Accounts.GetFreelancePtById;

public class GetFreelancePTByIdQueryHandler(IApplicationUserService _applicationUserService, IMapper _mapper, IUnitOfWork _unitOfWork, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<GetFreelancePTByIdQuery, GetFreelancePtByIdResponseDto>
{
    public async Task<GetFreelancePtByIdResponseDto> Handle(GetFreelancePTByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        var freelancePt = await _applicationUserService.GetByIdAsync(request.Id, includes: new List<string>
        {
            "UserDetail",
            "PTFreelancePackages",
            "GoalTrainings",
            "PtCertificates",
            "PtCertificates.CertificateMetadata"
        });
        if (freelancePt == null)
        {
            throw new NotFoundException("Freelance PT not found");
        }
        var freelancePtDto = _mapper.Map<GetFreelancePtByIdResponseDto>(freelancePt);
        var customerPurchasedSpec = new GetCustomerPurchasedByFreelancePtIdCountSpec(freelancePt.Id);
        var customerPurchaseds = await _unitOfWork.Repository<CustomerPurchased>().GetAllWithSpecificationAsync(customerPurchasedSpec);
        freelancePtDto.FreelancePTPackages = freelancePtDto.FreelancePTPackages.Where(x => x.IsEnabled).ToList();
        var totalPurchased = customerPurchaseds.Sum(x => x.OrderItems.Count);
        foreach (var package in freelancePtDto.FreelancePTPackages)
        {
            var packagePurchasedSpec = new GetCustomerPurchasedByPackageIdSpec(package.Id);
            var packagePurchased = await _unitOfWork.Repository<CustomerPurchased>().GetAllWithSpecificationAsync(packagePurchasedSpec);
            var packageTotalPurchased = packagePurchased.Sum(x => x.OrderItems.Count);
            if (userId != null)
            {
                package.IsPurchased = packagePurchased.Any(x => x.CustomerId == userId && x.IsEnabled && x.ExpirationDate >= DateOnly.FromDateTime(DateTime.UtcNow));
            }
            else
            {
                package.IsPurchased = false;
            }
            package.TotalPurchased = packageTotalPurchased;
        }
        freelancePtDto.FreelancePt.TotalPurchased = totalPurchased;
        return freelancePtDto;
    }
}
