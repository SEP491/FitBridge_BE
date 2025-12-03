using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitBridge_Application.Dtos.Certificates;
using MediatR;
using FitBridge_Application.Specifications;
using FitBridge_Application.Dtos;
using FitBridge_Application.Specifications.Certificates.GetCertificates;

namespace FitBridge_Application.Features.Certificates.GetCertificate;

public class GetCertificatesQuery : IRequest<PagingResultDto<CertificatesDto>>
{
    public GetCertificatesParams Params { get; set; }
}
