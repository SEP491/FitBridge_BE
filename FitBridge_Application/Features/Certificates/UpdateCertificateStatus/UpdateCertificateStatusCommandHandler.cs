using System;
using FitBridge_Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using FitBridge_Domain.Entities.Certificates;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Enums.Certificates;
using FitBridge_Application.Interfaces.Services;

namespace FitBridge_Application.Features.Certificates.UpdateCertificateStatus;

public class UpdateCertificateStatusCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IScheduleJobServices _scheduleJobServices) : IRequestHandler<UpdateCertificateStatusCommand, bool>
{
    public async Task<bool> Handle(UpdateCertificateStatusCommand request, CancellationToken cancellationToken)
    {
        var certificate = await _unitOfWork.Repository<PtCertificates>().GetByIdAsync(request.CertificateId);
        if (certificate == null)
        {
            throw new NotFoundException(nameof(PtCertificates));
        }
        certificate.CertificateStatus = request.CertificateStatus;
        if (request.CertificateStatus == CertificateStatus.Active)
        {
            if (certificate.ExpirationDate != null)
            {
                await _scheduleJobServices.ScheduleAutoExpiredCertificateJob(request.CertificateId, certificate.ExpirationDate.Value.ToDateTime(TimeOnly.MaxValue));
            }
        }
        _unitOfWork.Repository<PtCertificates>().Update(certificate);
        await _unitOfWork.CommitAsync();
        return true;
    }
}
