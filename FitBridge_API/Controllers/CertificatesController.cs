using System;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Certificates;
using FitBridge_Application.Features.Certificates.AddCertificateRequest;
using FitBridge_Application.Features.Certificates.DeleteCertificate;
using FitBridge_Application.Features.Certificates.GetCertificate;
using FitBridge_Application.Features.Certificates.GetCertificateMetadata;
using FitBridge_Application.Features.Certificates.UpdateCertificateStatus;
using FitBridge_Application.Specifications.Certificates.GetCertificates;
using FitBridge_Application.Specifications.Certificates.GetCertMetadata;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers;

public class CertificatesController(IMediator mediator) : _BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> AddCertificateRequest([FromForm] AddCertificateRequestCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Certificate request added successfully", result));
    }

    [HttpGet]
    public async Task<IActionResult> GetCertificates([FromQuery] GetCertificatesParams requestsParams)
    {
        var result = await mediator.Send(new GetCertificatesQuery { Params = requestsParams });
        var pagination = ResultWithPagination(result.Items, result.Total, requestsParams.Page, requestsParams.Size);
        return Ok(new BaseResponse<Pagination<CertificatesDto>>(StatusCodes.Status200OK.ToString(), "Certificates retrieved successfully", pagination));
    }

    [HttpPut("{certificateId}")]
    public async Task<IActionResult> UpdateCertificateStatus([FromRoute] Guid certificateId, [FromBody] UpdateCertificateStatusCommand command)
    {
        command.CertificateId = certificateId;
        var result = await mediator.Send(command);
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Certificate status updated successfully", result));
    }

    [HttpDelete("{certificateId}")]
    public async Task<IActionResult> DeleteCertificate([FromRoute] Guid certificateId)
    {
        var result = await mediator.Send(new DeleteCertificateCommand { CertificateId = certificateId });
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Certificate deleted successfully", result));
    }

    [HttpGet("metadata")]
    public async Task<IActionResult> GetCertificateMetadate([FromQuery] GetCertificateMetadataParams metadataParams) {
        var result = await mediator.Send(new GetCertificateMetadataQuery { Params = metadataParams });
        var pagination = ResultWithPagination(result.Items, result.Total, metadataParams.Page, metadataParams.Size);
        return Ok(new BaseResponse<Pagination<CertificateMetadataDto>>(StatusCodes.Status200OK.ToString(), "Certificate metadata retrieved successfully", pagination));
    }
}
