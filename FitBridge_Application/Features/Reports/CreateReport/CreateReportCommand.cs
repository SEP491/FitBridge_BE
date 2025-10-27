using FitBridge_Application.Dtos.Reports;
using FitBridge_Domain.Enums.Reports;
using MediatR;

namespace FitBridge_Application.Features.Reports.CreateReport
{
    public class CreateReportCommand : IRequest<CreateReportResponseDto>
    {
        public Guid ReportedItemId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public List<string> ImageUrls { get; set; }
    }
}