using System;
using FitBridge_Domain.Entities.Certificates;

namespace FitBridge_Application.Specifications.Certificates.GetCertMetadata;

public class GetCertificateMetadataSpec : BaseSpecification<CertificateMetadata>
{
    public GetCertificateMetadataSpec(GetCertificateMetadataParams metadataParams)
    : base(x =>
        (metadataParams.Id == null || x.Id == metadataParams.Id)
    )
    {
        if (metadataParams.SortOrder == "asc")
        {
            AddOrderBy(x => x.CertName);
        }
        else
        {
            AddOrderByDesc(x => x.CertName);
        }
        
        if (metadataParams.DoApplyPaging)
        {
            AddPaging(metadataParams.Size * (metadataParams.Page - 1), metadataParams.Size);
        }
        else
        {
            metadataParams.Size = -1;
            metadataParams.Page = -1;
        }
    }
}
