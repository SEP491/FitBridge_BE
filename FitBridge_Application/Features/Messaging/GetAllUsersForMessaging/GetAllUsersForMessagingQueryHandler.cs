using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Messaging;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Messaging.GetAllUsersForMessaging;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Messaging.GetAllUsersForMessaging;

internal class GetAllUsersForMessagingQueryHandler(
    IApplicationUserService applicationUserService,
    IHttpContextAccessor httpContextAccessor,
    IUserUtil userUtil) : IRequestHandler<GetAllUsersForMessagingQuery, PagingResultDto<MessagingUserDto>>
{
    public async Task<PagingResultDto<MessagingUserDto>> Handle(GetAllUsersForMessagingQuery request, CancellationToken cancellationToken)
    {
        var userId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                ?? throw new NotFoundException(nameof(ApplicationUser));

        var currentUser = await applicationUserService.GetByIdAsync(userId)
            ?? throw new NotFoundException(nameof(ApplicationUser));

        var currentUserRole = await applicationUserService.GetUserRoleAsync(currentUser);

        var spec = new GetAllUsersForMessagingSpec(request.Params, userId);
        var users = await applicationUserService.GetAllUsersWithSpecAsync(spec);

        var dtos = new List<MessagingUserDto>();

        foreach (var user in users)
        {
            var userRole = await applicationUserService.GetUserRoleAsync(user);
            
            // Exclude users with the same role as the current user
            if (userRole != currentUserRole)
            {
                dtos.Add(new MessagingUserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    IsMale = user.IsMale,
                    AvatarUrl = user.AvatarUrl ?? string.Empty,
                    UserRole = userRole
                });
            }
        }

        // Count only users with different roles
        var totalCount = dtos.Count;

        return new PagingResultDto<MessagingUserDto>(totalCount, dtos);
    }
}