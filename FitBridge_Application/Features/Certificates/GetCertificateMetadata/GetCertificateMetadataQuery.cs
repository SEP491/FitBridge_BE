using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Certificates;
using FitBridge_Application.Specifications.Certificates.GetCertMetadata;
using MediatR;

namespace FitBridge_Application.Features.Certificates.GetCertificateMetadata;

public class GetCertificateMetadataQuery : IRequest<PagingResultDto<CertificateMetadataDto>>
{
    public GetCertificateMetadataParams Params { get; set; }
}
