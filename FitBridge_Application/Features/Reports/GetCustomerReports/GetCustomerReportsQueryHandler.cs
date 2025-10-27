using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Reports;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Reports.GetCustomerReports;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Reports;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Reports.GetCustomerReports
{
    internal class GetCustomerReportsQueryHandler(
        IUnitOfWork unitOfWork,
        IUserUtil userUtil,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper) : IRequestHandler<GetCustomerReportsQuery, PagingResultDto<GetCustomerReportsResponseDto>>
    {
        public async Task<PagingResultDto<GetCustomerReportsResponseDto>> Handle(GetCustomerReportsQuery request, CancellationToken cancellationToken)
        {
            var userId = userUtil.GetAccountId(httpContextAccessor.HttpContext!)
                ?? throw new NotFoundException(nameof(ApplicationUser));

            var spec = new GetCustomerReportsSpec(request.Params, userId);

            var reports = await unitOfWork.Repository<ReportCases>()
       .GetAllWithSpecificationProjectedAsync<GetCustomerReportsResponseDto>(spec, mapper.ConfigurationProvider);

            var totalItems = await unitOfWork.Repository<ReportCases>()
                 .CountAsync(spec);

            return new PagingResultDto<GetCustomerReportsResponseDto>(totalItems, reports);
        }
    }
}