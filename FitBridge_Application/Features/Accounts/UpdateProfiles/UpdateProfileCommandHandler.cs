using AutoMapper;
using FitBridge_Application.Dtos.Accounts.Profiles;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Accounts.UpdateProfiles;

public class UpdateProfileCommandHandler(IApplicationUserService applicationUserService, IMapper _mapper, IUnitOfWork _unitOfWork, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<UpdateProfileCommand, UpdateProfileResponseDto>
{
    public async Task<UpdateProfileResponseDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new NotFoundException("User not found");
        }
        var account = await applicationUserService.GetByIdAsync(userId.Value, includes: new List<string> { "UserDetail" }, true);
        if (account == null)
        {
            throw new NotFoundException("Account not found");
        }
        try
        {
            if (request.UserDetail != null)
            {
                _mapper.Map(request.UserDetail, account.UserDetail);
            }
            account.FullName = request.FullName ?? account.FullName;
            account.AvatarUrl = request.AvatarUrl ?? account.AvatarUrl;
            account.IsMale = request.IsMale ?? account.IsMale;
            account.Longitude = request.Longitude ?? account.Longitude;
            account.Latitude = request.Latitude ?? account.Latitude;
            if(request.Dob != null)
            {
                account.Dob = DateTime.SpecifyKind(request.Dob.Value, DateTimeKind.Utc);
            }
            account.TaxCode = request.TaxCode ?? account.TaxCode;
            account.GymDescription = request.GymDescription ?? account.GymDescription;
            account.GymName = request.GymName ?? account.GymName;
            account.IdentityCardPlace = request.IdentityCardPlace ?? account.IdentityCardPlace;
            account.CitizenCardPermanentAddress = request.CitizenCardPermanentAddress ?? account.CitizenCardPermanentAddress;
            account.CitizenIdNumber = request.CitizenIdNumber ?? account.CitizenIdNumber;
            account.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            throw new BusinessException("Failed to update profile", ex);
        }

        return _mapper.Map<UpdateProfileResponseDto>(account);
    }
}
