using System;
using FitBridge_Application.Dtos.Accounts.UserDetails;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Accounts;
using MediatR;
using AutoMapper;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.UserDetails.GetUserDetail;

public class GetUserDetailsQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<GetUserDetailsQuery, UserDetailDto>
{
    public async Task<UserDetailDto> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext)
            ?? throw new NotFoundException(nameof(ApplicationUser));
        var userDetail = await _unitOfWork.Repository<UserDetail>().GetByIdAsync(userId);
        return _mapper.Map<UserDetailDto>(userDetail);
    }
}
