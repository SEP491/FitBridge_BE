using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos.Reports;
using FitBridge_Application.Features.Reports.CreateReport;
using FitBridge_Application.Features.Reports.GetAllReports;
using FitBridge_Application.Features.Reports.GetCustomerReports;
using FitBridge_Application.Features.Reports.GetReportById;
using FitBridge_Application.Features.Reports.UpdateReportStatus;
using FitBridge_Application.Specifications.Reports.GetAllReports;
using FitBridge_Application.Specifications.Reports.GetCustomerReports;
using FitBridge_Domain.Enums.Reports;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers
{
    /// <summary>
    /// Controller for managing report cases, including creating, retrieving, and updating report statuses.
    /// </summary>
    [Authorize]
    public class ReportsController(IMediator mediator) : _BaseApiController
    {
        /// <summary>
        /// Creates a new report case.
        /// </summary>
        /// <param name="command">The report creation details including:
        /// <list type="bullet">
        /// <item>
        /// <term>ReportedItemId</term>
        /// <description>The ID of the order item being reported.</description>
        /// </item>
        /// <item>
        /// <term>Title</term>
        /// <description>The title of the report.</description>
        /// </item>
        /// <item>
        /// <term>Description</term>
        /// <description>Detailed description of the issue.</description>
        /// </item>
        /// <item>
        /// <term>ReportType</term>
        /// <description>Type of report (ProductReport, FreelancePtReport, or GymCourseReport).</description>
        /// </item>
        /// <item>
        /// <term>ImageUrls</term>
        /// <description>List of image URLs as evidence.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>The created report ID.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<CreateReportResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CreateReportResponseDto>> CreateReport([FromBody] CreateReportCommand command)
        {
            var response = await mediator.Send(command);

            return Ok(
new BaseResponse<CreateReportResponseDto>(
      StatusCodes.Status200OK.ToString(),
     "Report created successfully",
          response));
        }

        /// <summary>
        /// Retrieves all reports with optional filtering and pagination.
        /// This endpoint is typically for admin users to view all reports in the system.
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination, including:
        /// <list type="bullet">
        /// <item>
        /// <term>Status</term>
        /// <description>Filter by report status (Pending, Processing, Resolved, FraudConfirmed).</description>
        /// </item>
        /// <item>
        /// <term>ReportType</term>
        /// <description>Filter by report type (ProductReport, FreelancePtReport, GymCourseReport).</description>
        /// </item>
        /// <item>
        /// <term>ReporterId</term>
        /// <description>Filter by reporter user ID.</description>
        /// </item>
        /// <item>
        /// <term>ReportedUserId</term>
        /// <description>Filter by reported user ID.</description>
        /// </item>
        /// <item>
        /// <term>SearchTerm</term>
        /// <description>Search in title, description, or user names.</description>
        /// </item>
        /// <item>
        /// <term>Page</term>
        /// <description>The page number to retrieve (default: 1).</description>
        /// </item>
        /// <item>
        /// <term>Size</term>
        /// <description>The number of items per page (default: 10, max: 20).</description>
        /// </item>
        /// <item>
        /// <term>SortBy</term>
        /// <description>The field to sort by (CreatedAt, Status, ReportType).</description>
        /// </item>
        /// <item>
        /// <term>SortOrder</term>
        /// <description>The sort direction (asc or desc).</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>A paginated list of all reports.</returns>
        [HttpGet]
        [Authorize(Roles = ProjectConstant.UserRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<Pagination<GetCustomerReportsResponseDto>>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Pagination<GetCustomerReportsResponseDto>>> GetAllReports(
            [FromQuery] GetAllReportsParams parameters)
        {
            var response = await mediator.Send(new GetAllReportsQuery { Params = parameters });

            var pagedResult = new Pagination<GetCustomerReportsResponseDto>(
                 response.Items,
                response.Total,
                  parameters.Page,
              parameters.Size);

            return Ok(
                new BaseResponse<Pagination<GetCustomerReportsResponseDto>>(
          StatusCodes.Status200OK.ToString(),
    "Get all reports success",
         pagedResult));
        }

        /// <summary>
        /// Retrieves reports created by the current customer.
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination, including:
        /// <list type="bullet">
        /// <item>
        /// <term>CustomerId</term>
        /// <description>The ID of the customer (reporter).</description>
        /// </item>
        /// <item>
        /// <term>SearchTerm</term>
        /// <description>Search in report title or description.</description>
        /// </item>
        /// <item>
        /// <term>Page</term>
        /// <description>The page number to retrieve (default: 1).</description>
        /// </item>
        /// <item>
        /// <term>Size</term>
        /// <description>The number of items per page (default: 10, max: 20).</description>
        /// </item>
        /// <item>
        /// <term>SortBy</term>
        /// <description>The field to sort by (CreatedAt, Status).</description>
        /// </item>
        /// <item>
        /// <term>SortOrder</term>
        /// <description>The sort direction (asc or desc).</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>A paginated list of reports created by the customer.</returns>
        [HttpGet("customer")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<Pagination<GetCustomerReportsResponseDto>>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Pagination<GetCustomerReportsResponseDto>>> GetCustomerReports(
            [FromQuery] GetCustomerReportsParams parameters)
        {
            var response = await mediator.Send(new GetCustomerReportsQuery { Params = parameters });

            var pagedResult = new Pagination<GetCustomerReportsResponseDto>(
              response.Items,
                response.Total,
                       parameters.Page,
             parameters.Size);

            return Ok(
               new BaseResponse<Pagination<GetCustomerReportsResponseDto>>(
          StatusCodes.Status200OK.ToString(),
                "Get customer reports success",
          pagedResult));
        }

        /// <summary>
        /// Retrieves the details of a specific report by its ID.
        /// </summary>
        /// <param name="reportId">The unique identifier of the report.</param>
        /// <returns>The details of the specified report including reporter and reported user information.</returns>
        [HttpGet("{reportId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<GetCustomerReportsResponseDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetCustomerReportsResponseDto>> GetReportById([FromRoute] Guid reportId)
        {
            var response = await mediator.Send(new GetReportByIdQuery { ReportId = reportId });

            return Ok(
     new BaseResponse<GetCustomerReportsResponseDto>(
          StatusCodes.Status200OK.ToString(),
        "Get report detail success",
   response));
        }

        /// <summary>
        /// Updates the status of an existing report case.
        /// Only admins can update report statuses.
        /// </summary>
        /// <param name="reportId">The unique identifier of the report to update.</param>
        /// <param name="command">The update details including:
        /// <list type="bullet">
        /// <item>
        /// <term>Status</term>
        /// <description>The new status (Pending, Processing, Resolved, FraudConfirmed).</description>
        /// </item>
        /// <item>
        /// <term>Note</term>
        /// <description>Optional note about the status update (required for FraudConfirmed status).</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>Success response if the status was updated.</returns>
        [HttpPut("{reportId}/status")]
        [Authorize(Roles = ProjectConstant.UserRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<object>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateReportStatus(
        [FromRoute] Guid reportId,
            [FromBody] UpdateReportStatusCommand command)
        {
            command.ReportId = reportId;
            await mediator.Send(command);

            return Ok(
              new BaseResponse<object>(
                   StatusCodes.Status200OK.ToString(),
                   "Report status updated successfully",
                 null));
        }
    }
}