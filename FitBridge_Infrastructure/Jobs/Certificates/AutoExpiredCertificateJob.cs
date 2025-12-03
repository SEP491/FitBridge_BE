using System;
using FitBridge_Domain.Exceptions;
using Quartz;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Certificates;
using FitBridge_Domain.Enums.Certificates;

namespace FitBridge_Infrastructure.Jobs.Certificates;

public class AutoExpiredCertificateJob(IUnitOfWork _unitOfWork)   : IJob   
{
    public async Task Execute(IJobExecutionContext context)
    {
        var certificateId = Guid.Parse(context.JobDetail.JobDataMap.GetString("certificateId")
            ?? throw new NotFoundException($"{nameof(AutoExpiredCertificateJob)}_certificateId"));
        var certificate = await _unitOfWork.Repository<PtCertificates>().GetByIdAsync(certificateId);
        if (certificate == null)
        {
            throw new NotFoundException(nameof(PtCertificates));
        }
        certificate.CertificateStatus = CertificateStatus.Expired;
        _unitOfWork.Repository<PtCertificates>().Update(certificate);
        await _unitOfWork.CommitAsync();
    }
}
