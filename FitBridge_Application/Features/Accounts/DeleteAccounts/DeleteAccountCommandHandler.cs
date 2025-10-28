using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Accounts.GetAllGymPts;
using FitBridge_Application.Specifications.Accounts.GetUsersByIds;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Accounts.DeleteAccounts
{
    internal class DeleteAccountCommandHandler(
        IApplicationUserService applicationUserService,
        IUserUtil userUtil,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<DeleteAccountCommand>
    {
        public async Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            var userId = userUtil.GetAccountId(httpContextAccessor.HttpContext!)
             ?? throw new NotFoundException(nameof(ApplicationUser));
            var currentUser = await applicationUserService.GetByIdAsync(userId);

            var userRole = await applicationUserService.GetUserRoleAsync(currentUser);

            if (userRole.Equals(ProjectConstant.UserRoles.Admin))
            {
                await DeleteAccounts(request.UserIdDeleteList);
            }
            else
            {
                await DeleteGymPts(request.UserIdDeleteList, userId);
            }
        }

        private async Task DeleteAccounts(List<Guid> userIdDeleteList)
        {
            var spec = new GetUsersByIdsSpec(userIdDeleteList);
            var users = await applicationUserService.GetAllUsersWithSpecAsync(spec, asNoTracking: false);

            List<Task> updateTasks = [];
            foreach (var user in users)
            {
                user.IsEnabled = false;
                updateTasks.Add(applicationUserService.UpdateAsync(user));
            }

            await Task.WhenAll(updateTasks);
        }

        private async Task DeleteGymPts(List<Guid> userIdDeleteList, Guid gymOwnerId)
        {
            var spec = new GetAllGymPtsSpec(userIdDeleteList, gymOwnerId);
            var users = await applicationUserService.GetAllUsersWithSpecAsync(spec, asNoTracking: false);

            List<Task> updateTasks = [];
            foreach (var user in users)
            {
                user.IsEnabled = false;
                updateTasks.Add(applicationUserService.UpdateAsync(user));
            }

            await Task.WhenAll(updateTasks);
        }
    }
}