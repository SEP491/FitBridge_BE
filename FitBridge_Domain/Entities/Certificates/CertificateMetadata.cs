using System;
using FitBridge_Domain.Enums.Certificates;

namespace FitBridge_Domain.Entities.Certificates;

public class CertificateMetadata : BaseEntity
{
    public List<string> Specializations { get; set; } = new List<string>();
    public CertificateType CertificateType { get; set; }
    public string CertCode { get; set; }
    public string CertName { get; set; }
    public string ProviderName { get; set; }
    public string Description { get; set; }
    public ICollection<PtCertificates> PtCertificates { get; set; } = new List<PtCertificates>();
}
