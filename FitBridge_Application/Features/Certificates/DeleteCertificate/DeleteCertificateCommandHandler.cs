using System;
using MediatR;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Certificates;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Interfaces.Services;

namespace FitBridge_Application.Features.Certificates.DeleteCertificate;

public class DeleteCertificateCommandHandler(IUnitOfWork _unitOfWork, IScheduleJobServices _scheduleJobServices, IUploadService _uploadService) : IRequestHandler<DeleteCertificateCommand, bool>
{
    public async Task<bool> Handle(DeleteCertificateCommand request, CancellationToken cancellationToken)
    {
        var certificate = await _unitOfWork.Repository<PtCertificates>().GetByIdAsync(request.CertificateId);
        if (certificate == null)
        {
            throw new NotFoundException(nameof(PtCertificates));
        }
        if(certificate.ExpirationDate == null)
        {
            await _scheduleJobServices.CancelScheduleJob($"AutoExpiredCertificate_{request.CertificateId}", "AutoExpiredCertificate");
        }
        if(certificate.CertUrl != null)
        {
            await _uploadService.DeleteFileAsync(certificate.CertUrl);
        }
        _unitOfWork.Repository<PtCertificates>().Delete(certificate);
        await _unitOfWork.CommitAsync();
        return true;
    }
}
