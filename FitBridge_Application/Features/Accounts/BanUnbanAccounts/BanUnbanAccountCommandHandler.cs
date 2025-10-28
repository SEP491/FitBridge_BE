using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Accounts.GetAllGymPts;
using FitBridge_Application.Specifications.Accounts.GetUsersByIds;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Accounts.BanUnbanAccounts
{
    internal class BanUnbanAccountCommandHandler(
        IApplicationUserService applicationUserService,
        IUserUtil userUtil,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<BanUnbanAccountCommand>
    {
        public async Task Handle(BanUnbanAccountCommand request, CancellationToken cancellationToken)
        {
            var userId = userUtil.GetAccountId(httpContextAccessor.HttpContext!)
                ?? throw new NotFoundException(nameof(ApplicationUser));
            var currentUser = await applicationUserService.GetByIdAsync(userId);

            var userRole = await applicationUserService.GetUserRoleAsync(currentUser);

            if (userRole.Equals(ProjectConstant.UserRoles.Admin))
            {
                await BanUnbanAccounts(request.UserIdBanUnbanList, request.IsBan);
            }
            else
            {
                await BanUnbanGymPts(request.UserIdBanUnbanList, request.IsBan, userId);
            }
        }

        private async Task BanUnbanAccounts(List<Guid> userIdBanList, bool isBan)
        {
            var spec = new GetUsersByIdsSpec(userIdBanList, isIncludeBanned: true);
            var users = await applicationUserService.GetAllUsersWithSpecAsync(spec, asNoTracking: false);

            if (users.Count == 0)
            {
                throw new NotFoundException(nameof(ApplicationUser));
            }

            List<Task> updateTasks = [];
            foreach (var user in users)
            {
                user.IsActive = !isBan;
                updateTasks.Add(applicationUserService.UpdateAsync(user));
            }

            await Task.WhenAll(updateTasks);
        }

        private async Task BanUnbanGymPts(List<Guid> userIdBanList, bool isBan, Guid gymOwnerId)
        {
            var spec = new GetAllGymPtsSpec(userIdBanList, gymOwnerId, isIncludeBanned: true);
            var users = await applicationUserService.GetAllUsersWithSpecAsync(spec, asNoTracking: false);

            if (users.Count == 0)
            {
                throw new NotFoundException(nameof(ApplicationUser));
            }

            List<Task> updateTasks = [];
            foreach (var user in users)
            {
                user.IsActive = !isBan;
                updateTasks.Add(applicationUserService.UpdateAsync(user));
            }

            await Task.WhenAll(updateTasks);
        }
    }
}