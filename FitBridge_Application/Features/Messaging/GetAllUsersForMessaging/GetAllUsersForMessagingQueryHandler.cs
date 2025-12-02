using FitBridge_Application.Commons.Constants;
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

        // Determine which roles to fetch
        List<string> rolesToFetch;
        if (request.Params.RoleFilter != null && request.Params.RoleFilter.Count > 0)
        {
            // Use specified roles, but exclude current user's role
            rolesToFetch = request.Params.RoleFilter
                .Where(r => r != currentUserRole)
                .ToList();
        }
        else
        {
            // Fetch all roles except the current user's role
            var allRoles = new List<string>
            {
                ProjectConstant.UserRoles.Admin,
                ProjectConstant.UserRoles.Customer,
                ProjectConstant.UserRoles.FreelancePT,
                ProjectConstant.UserRoles.GymPT,
                ProjectConstant.UserRoles.GymOwner
            };
            rolesToFetch = allRoles.Where(r => r != currentUserRole).ToList();
        }

        // If no roles to fetch, return empty result
        if (rolesToFetch.Count == 0)
        {
            return new PagingResultDto<MessagingUserDto>(0, new List<MessagingUserDto>());
        }

        // Fetch users from all eligible roles
        var allUsersInRoles = new List<ApplicationUser>();
        foreach (var role in rolesToFetch)
        {
            var usersInRole = await applicationUserService.GetUsersByRoleAsync(role);
            allUsersInRoles.AddRange(usersInRole);
        }

        // Apply specification filters (search, sorting, etc.) on the combined list
        var spec = new GetAllUsersForMessagingSpec(request.Params, userId);
        var filteredUsers = allUsersInRoles
            .Where(u => u.Id != userId &&
                       (string.IsNullOrEmpty(request.Params.SearchTerm) ||
                        u.FullName.ToLower().Contains(request.Params.SearchTerm.ToLower()) ||
                        (u.Email != null && u.Email.ToLower().Contains(request.Params.SearchTerm.ToLower()))))
            .ToList();

        // Apply sorting
        var sortedUsers = request.Params.SortOrder.ToLower() == "desc"
            ? filteredUsers.OrderByDescending(u => u.FullName).ToList()
            : filteredUsers.OrderBy(u => u.FullName).ToList();

        var totalCount = sortedUsers.Count;

        // Apply pagination
        var pagedUsers = request.Params.DoApplyPaging
            ? sortedUsers
                .Skip((request.Params.Page - 1) * request.Params.Size)
                .Take(request.Params.Size)
                .ToList()
            : sortedUsers;

        // Map to DTOs
        var dtos = new List<MessagingUserDto>();
        foreach (var user in pagedUsers)
        {
            var userRole = await applicationUserService.GetUserRoleAsync(user);
            dtos.Add(new MessagingUserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                IsMale = user.IsMale,
                AvatarUrl = user.AvatarUrl ?? string.Empty,
                UserRole = userRole
            });
        }

        return new PagingResultDto<MessagingUserDto>(totalCount, dtos);
    }
}