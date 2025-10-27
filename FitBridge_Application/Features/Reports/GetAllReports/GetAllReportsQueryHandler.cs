using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Reports;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Reports.GetAllReports;
using FitBridge_Domain.Entities.Reports;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FitBridge_Application.Features.Reports.GetAllReports
{
    internal class GetAllReportsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetAllReportsQuery, PagingResultDto<GetCustomerReportsResponseDto>>
    {
        public async Task<PagingResultDto<GetCustomerReportsResponseDto>> Handle(GetAllReportsQuery request, CancellationToken cancellationToken)
        {
            var spec = new GetAllReportsSpec(request.Params);

            var reports = await unitOfWork.Repository<ReportCases>()
                .GetAllWithSpecificationProjectedAsync<GetCustomerReportsResponseDto>(spec, mapper.ConfigurationProvider);

            var totalItems = await unitOfWork.Repository<ReportCases>()
                .CountAsync(spec);

            return new PagingResultDto<GetCustomerReportsResponseDto>(totalItems, reports);
        }
    }
}