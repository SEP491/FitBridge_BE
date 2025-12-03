using System;
using FitBridge_Domain.Enums.Certificates;

namespace FitBridge_Application.Dtos.Certificates;

public class CertificatesDto
{
    public Guid Id { get; set; }
    public Guid PtId { get; set; }
    public string? PtName { get; set; }
    public string? PtImageUrl { get; set; }
    public Guid CertificateMetadataId { get; set; }
    public string CertUrl { get; set; }
    public DateOnly ProvidedDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public CertificateStatus CertificateStatus { get; set; }
    public CertificateMetadataDto CertificateMetadata { get; set; }
}
