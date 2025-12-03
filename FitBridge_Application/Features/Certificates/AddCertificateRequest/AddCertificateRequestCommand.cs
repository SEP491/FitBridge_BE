using System;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Certificates.AddCertificateRequest;

public class AddCertificateRequestCommand : IRequest<bool>
{
    public Guid PtId { get; set; }
    public Guid CertificateMetadataId { get; set; }
    public IFormFile CertUrl { get; set; }
    public DateOnly ProvidedDate {get;set;}
    public DateOnly? ExpirationDate{get;set;}
}
