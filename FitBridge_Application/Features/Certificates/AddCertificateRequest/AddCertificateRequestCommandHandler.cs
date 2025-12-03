using System;
using MediatR;
using FitBridge_Application.Interfaces.Repositories;
using AutoMapper;
using FitBridge_Domain.Entities.Certificates;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Enums.Certificates;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Commons.Constants;

namespace FitBridge_Application.Features.Certificates.AddCertificateRequest;

public class AddCertificateRequestCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor, IUploadService _uploadService, IApplicationUserService _applicationUserService) : IRequestHandler<AddCertificateRequestCommand, bool>
{
    public async Task<bool> Handle(AddCertificateRequestCommand request, CancellationToken cancellationToken)
    {
        var user = await _applicationUserService.GetByIdAsync(request.PtId) ?? throw new NotFoundException("User not found");
        var role = await _applicationUserService.GetUserRoleAsync(user);
        if(role != ProjectConstant.UserRoles.FreelancePT)
        {
            throw new BusinessException("User is not a Freelance PT");
        }
        var mappedEntity = _mapper.Map<AddCertificateRequestCommand, PtCertificates>(request);
        mappedEntity.CertificateStatus = CertificateStatus.WaitingForReview;
        mappedEntity.CertUrl = await _uploadService.UploadFileAsync(request.CertUrl);
        _unitOfWork.Repository<PtCertificates>().Insert(mappedEntity);
        await _unitOfWork.CommitAsync();
        return true;
    }
}
