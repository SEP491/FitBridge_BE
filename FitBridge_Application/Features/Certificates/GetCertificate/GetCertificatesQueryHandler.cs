using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Certificates;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using AutoMapper;
using FitBridge_Application.Specifications.Certificates.GetCertificates;
using FitBridge_Domain.Entities.Certificates;

namespace FitBridge_Application.Features.Certificates.GetCertificate;

public class GetCertificatesQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetCertificatesQuery, PagingResultDto<CertificatesDto>>
{
    public async Task<PagingResultDto<CertificatesDto>> Handle(GetCertificatesQuery request, CancellationToken cancellationToken)
    {
        var certificates = await _unitOfWork.Repository<PtCertificates>().GetAllWithSpecificationAsync(new GetCertificatesSpec(request.Params));
        var certificatesDtos = _mapper.Map<IReadOnlyList<CertificatesDto>>(certificates);
        var totalItems = await _unitOfWork.Repository<PtCertificates>().CountAsync(new GetCertificatesSpec(request.Params));
        return new PagingResultDto<CertificatesDto>(totalItems, certificatesDtos);
    }
}
