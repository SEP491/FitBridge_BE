using System;

namespace FitBridge_Application.Specifications.Certificates.GetCertMetadata;

public class GetCertificateMetadataParams : BaseParams
{
    public Guid? Id { get; set; }
}
