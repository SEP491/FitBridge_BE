using FitBridge_Application.Specifications;
using FitBridge_Domain.Enums.Certificates;

namespace FitBridge_Application.Specifications.Certificates.GetCertificates;

public class GetCertificatesParams : BaseParams
{
    public Guid? PtId { get; set; }
    public Guid? CertificateId { get; set; }
    public CertificateStatus? CertificateStatus { get; set; }
}
