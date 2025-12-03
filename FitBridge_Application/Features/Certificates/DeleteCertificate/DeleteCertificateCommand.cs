using System;
using MediatR;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.Certificates.DeleteCertificate;

public class DeleteCertificateCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid CertificateId { get; set; }
}
