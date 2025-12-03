using AutoMapper;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos.Accounts.Profiles;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Accounts.CheckAccountUpdateData;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace FitBridge_Application.Features.Accounts.UpdateProfiles;

public class UpdateProfileCommandHandler(IApplicationUserService applicationUserService, IMapper _mapper, IUnitOfWork _unitOfWork, 
    IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor, IUploadService _uploadService) : IRequestHandler<UpdateProfileCommand, UpdateProfileResponseDto>
{
    public async Task<UpdateProfileResponseDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var account = await applicationUserService.GetByIdAsync(request.Id.Value, includes: new List<string> { "UserDetail" }, true);
        if (account == null)
        {
            throw new NotFoundException("Account not found");
        }
        
        await validateUpdateProfile(request);
        try
        {
            if (request.FrontCitizenIdFile != null)
            {
                var uploadedUrl = await _uploadService.UploadFileAsync(request.FrontCitizenIdFile);
                if (account.FrontCitizenIdUrl != null)
                {
                    await _uploadService.DeleteFileAsync(account.FrontCitizenIdUrl);
                }
                account.FrontCitizenIdUrl = uploadedUrl;
            }
            if (request.BackCitizenIdFile != null)
            {
                var uploadedUrl = await _uploadService.UploadFileAsync(request.BackCitizenIdFile);
                if (account.BackCitizenIdUrl != null)
                {
                    await _uploadService.DeleteFileAsync(account.BackCitizenIdUrl);
                }
                account.BackCitizenIdUrl = uploadedUrl;
            }
            account.FullName = request.FullName ?? account.FullName;
            account.AvatarUrl = request.AvatarUrl ?? account.AvatarUrl;
            account.IsMale = request.IsMale ?? account.IsMale;
            account.Longitude = request.Longitude ?? account.Longitude;
            account.Latitude = request.Latitude ?? account.Latitude;
            if (request.Dob != null)
            {
                account.Dob = DateTime.SpecifyKind(request.Dob.Value, DateTimeKind.Utc);
            }
            if (request.Bio != null && account.UserDetail != null)
            {
                account.UserDetail.Bio = request.Bio;
                account.UserDetail.UpdatedAt = DateTime.UtcNow;
            }
            account.TaxCode = request.TaxCode ?? account.TaxCode;
            account.GymDescription = request.GymDescription ?? account.GymDescription;
            account.GymName = request.GymName ?? account.GymName;
            account.IdentityCardPlace = request.IdentityCardPlace ?? account.IdentityCardPlace;
            account.CitizenCardPermanentAddress = request.CitizenCardPermanentAddress ?? account.CitizenCardPermanentAddress;
            account.CitizenIdNumber = request.CitizenIdNumber ?? account.CitizenIdNumber;
            account.UpdatedAt = DateTime.UtcNow;
            account.GymFoundationDate = request.GymFoundationDate ?? account.GymFoundationDate;
            account.IdentityCardDate = request.IdentityCardDate ?? account.IdentityCardDate;
            account.BusinessAddress = request.BusinessAddress ?? account.BusinessAddress;
            account.OpenTime = request.OpenTime ?? account.OpenTime;
            account.CloseTime = request.CloseTime ?? account.CloseTime;
            await HandleImagesUpdate(account, request);
            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            throw new BusinessException("Failed to update profile", ex);
        }

        return _mapper.Map<UpdateProfileResponseDto>(account);
    }

    public async Task validateUpdateProfile(UpdateProfileCommand request)
    {
        if(request.OpenTime != null && request.CloseTime != null)
        {
            if(request.OpenTime >= request.CloseTime)
            {
                throw new BusinessException("Open time must be before close time");
            }
        }
        if(request.TaxCode != null)
        {
            var spec = new CheckAccountUpdateSpec(request.Id.Value, request.TaxCode, null);
            var existingUser = await applicationUserService.CountAsync(spec);
            if (existingUser > 0)
            {
                throw new DuplicateUserException("Tax code already exists");
            }
        }
        if(request.CitizenIdNumber != null)
        {
            var spec = new CheckAccountUpdateSpec(request.Id.Value, null, request.CitizenIdNumber);
            var existingUser = await applicationUserService.CountAsync(spec);
            if (existingUser > 0)
            {
                throw new DuplicateUserException("Citizen id number already exists");
            }
        }
    }

    private async Task HandleImagesUpdate(ApplicationUser account, UpdateProfileCommand request)
    {
        var role = await applicationUserService.GetUserRoleAsync(account);
        if (role == ProjectConstant.UserRoles.FreelancePT)
        {
            if (request.ImagesToRemove != null && request.ImagesToRemove.Any())
            {
                if (account.FreelancePtImages == null || account.FreelancePtImages.Count == 0)
                {
                    throw new BusinessException("No images to remove");
                }
                foreach (var imageUrl in request.ImagesToRemove)
                {
                    if (account.FreelancePtImages.Contains(imageUrl))
                    {
                        // Delete from storage
                        await _uploadService.DeleteFileAsync(imageUrl);
                        // Remove from list
                        account.FreelancePtImages.Remove(imageUrl);
                    }
                }
            }

            // Upload and add new images
            if (request.ImagesToAdd != null && request.ImagesToAdd.Any())
            {
                foreach (var file in request.ImagesToAdd)
                {
                    var uploadedUrl = await _uploadService.UploadFileAsync(file);
                    account.FreelancePtImages.Add(uploadedUrl);
                }
            }
        }
        
        if (role == ProjectConstant.UserRoles.GymOwner)
        {
            if (request.ImagesToRemove != null && request.ImagesToRemove.Any())
            {
                if (account.GymImages == null || account.GymImages.Count == 0)
                {
                    throw new BusinessException("No images to remove");
                }
                foreach (var imageUrl in request.ImagesToRemove)
                {
                    if (account.GymImages.Contains(imageUrl))
                    {
                        // Delete from storage
                        await _uploadService.DeleteFileAsync(imageUrl);
                        // Remove from list
                        account.GymImages.Remove(imageUrl);
                    }
                }
            }

            // Upload and add new images
            if (request.ImagesToAdd != null && request.ImagesToAdd.Any())
            {
                foreach (var file in request.ImagesToAdd)
                {
                    var uploadedUrl = await _uploadService.UploadFileAsync(file);
                    account.GymImages.Add(uploadedUrl);
                }
            }
        }
    }
}
