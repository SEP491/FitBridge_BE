using System;
using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Certificates;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Certificates.GetCertMetadata;
using FitBridge_Domain.Entities.Certificates;
using MediatR;

namespace FitBridge_Application.Features.Certificates.GetCertificateMetadata;

public class GetCertificateMetadataQueryHandler(IUnitOfWork unitOfWork, IMapper _mapper) : IRequestHandler<GetCertificateMetadataQuery, PagingResultDto<CertificateMetadataDto>> 
{
    public async Task<PagingResultDto<CertificateMetadataDto>> Handle(GetCertificateMetadataQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetCertificateMetadataSpec(request.Params);
        var certificates = await unitOfWork.Repository<CertificateMetadata>().GetAllWithSpecificationAsync(spec);
        var result = _mapper.Map<IReadOnlyList<CertificateMetadataDto>>(certificates);
        var totalItems = await unitOfWork.Repository<CertificateMetadata>().CountAsync(spec);
        return new PagingResultDto<CertificateMetadataDto>(totalItems, result);
    }
}
