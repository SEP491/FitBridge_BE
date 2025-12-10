using System;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Enums.Certificates;

namespace FitBridge_Domain.Entities.Certificates;

public class PtCertificates : BaseEntity
{
    public Guid PtId { get; set; }
    public Guid CertificateMetadataId { get; set; }
    public string CertUrl { get; set; }
    public DateOnly ProvidedDate {get;set;}
    public DateOnly? ExpirationDate{get;set;}
    public CertificateStatus CertificateStatus { get; set; }
    public string? Note { get; set; }
    public CertificateMetadata CertificateMetadata { get; set; }

    public ApplicationUser Pt { get; set; }

}
