using System;
using MediatR;
using FitBridge_Domain.Enums.Certificates;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.Certificates.UpdateCertificateStatus;

public class UpdateCertificateStatusCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid CertificateId { get; set; }
    public CertificateStatus CertificateStatus { get; set; }
    public string Note { get; set; }
}
