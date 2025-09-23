using System;
using FitBridge_Application.Dtos.Accounts;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Specifications.Accounts;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Accounts.GetUserProfile;

public class GetProfileCommandHandler(IApplicationUserService applicationUserService) : IRequestHandler<GetProfileCommand, GetUserProfileResponse>
{
    public async Task<GetUserProfileResponse> Handle(GetProfileCommand request, CancellationToken cancellationToken)
    {
        var account = await applicationUserService.GetUserWithSpecAsync(new GetAccountByIdSpecificationForUserProfile(request.AccountId ?? Guid.Empty));
        if (account == null)
        {
            throw new NotFoundException("Account not found");
        }
        var addressString = "";
        if (account.Addresses.Count > 0)
        {
            Address address = account.Addresses.First();
            addressString = $"{address.Street}, {address.Ward}, {address.District}, {address.City}";
        }

        return new GetUserProfileResponse
        {
            FullName = account.FullName,
            Email = account.Email,
            Phone = account.PhoneNumber,
            DOB = account.Dob,
            Weight = account.UserDetail.Weight,
            Height = account.UserDetail.Height,
            Gender = account.IsMale ? "Male" : "Female",
            Address = addressString,
            AvatarUrl = account.AvatarUrl,
        };
    }
}