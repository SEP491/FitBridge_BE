using AutoMapper;
using FitBridge_Application.Dtos.Reports;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Reports.GetReportById;
using FitBridge_Domain.Entities.Reports;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Reports.GetReportById
{
    internal class GetReportByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<GetReportByIdQuery, GetCustomerReportsResponseDto>
    {
        public async Task<GetCustomerReportsResponseDto> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
        {
            var spec = new GetReportByIdSpec(request.ReportId);

            var report = await unitOfWork.Repository<ReportCases>()
                .GetBySpecificationProjectedAsync<GetCustomerReportsResponseDto>(spec, mapper.ConfigurationProvider)
                ?? throw new NotFoundException(nameof(ReportCases), request.ReportId);

            return report;
        }
    }
}