using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Certificates;

namespace FitBridge_Application.Specifications.Certificates.GetCertificates;

public class GetCertificatesSpec : BaseSpecification<PtCertificates>
{
    public GetCertificatesSpec(GetCertificatesParams parameters)
    : base(x => (parameters.PtId == null || x.PtId == parameters.PtId)
    && (parameters.CertificateStatus == null || x.CertificateStatus == parameters.CertificateStatus)
    && (parameters.CertificateId == null || x.Id == parameters.CertificateId)
    )
    {
        AddInclude(x => x.CertificateMetadata);
        AddInclude(x => x.Pt);

        if (parameters.SortBy == "asc")
        {
            AddOrderBy(x => x.CreatedAt);
        }
        else if (parameters.SortBy == "desc")
        {
            AddOrderByDesc(x => x.CreatedAt);
        }
        if (parameters.DoApplyPaging)
        {
            AddPaging((parameters.Page - 1) * parameters.Size, parameters.Size);
        }
        else
        {
            parameters.Size = -1;
            parameters.Page = -1;
        }
    }
}