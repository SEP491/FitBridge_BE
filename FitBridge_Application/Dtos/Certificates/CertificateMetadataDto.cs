using System;
using FitBridge_Domain.Enums.Certificates;

namespace FitBridge_Application.Dtos.Certificates;

public class CertificateMetadataDto
{
    public Guid Id { get; set; }
    public List<string> Specializations { get; set; } = new List<string>();
    public CertificateType CertificateType { get; set; }
    public string CertCode { get; set; }
    public string CertName { get; set; }
    public string ProviderName { get; set; }
    public string Description { get; set; }
}
