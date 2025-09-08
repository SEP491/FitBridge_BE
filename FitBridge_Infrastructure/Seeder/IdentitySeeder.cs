using FitBridge_Application.Interfaces.Utils.Seeding;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FitBridge_Infrastructure.Seeder;

public class IdentitySeeder(
    ILogger<IdentitySeeder> logger,
    FitBridgeDbContext dbContext,
    UserManager<ApplicationUser> userManager) : IIdentitySeeder
{
    public async Task SeedAsync()
    {
        if (await dbContext.Database.CanConnectAsync())
            try
            {
                IEnumerable<ApplicationUser>? users = null;
                if (!dbContext.Roles.Any())
                {
                    var roles = IdentityData.GetRoles();
                    dbContext.Roles.AddRange(roles);
                    await dbContext.SaveChangesAsync();
                }
                if (!dbContext.Users.Any())
                {
                    users = IdentityData.GetUsers();
                    foreach (var user in users) await userManager.CreateAsync(user, "Password1!");
                }
                if (users is not null && !dbContext.UserRoles.Any()) await GetUserRoles(users);
            }
            catch (Exception ex)
            {
                logger.LogError("{Ex} error seeding", ex);
            }
    }

    private async Task GetUserRoles(IEnumerable<ApplicationUser> users)
    {
        foreach (var user in users)
        {
            //student one -> student
            //var strippedUserRole = user.FirstName.ToLower().Split(' ')[0];
            //switch (strippedUserRole)
            //{
            //case "jobseeker":
            //    await userManager.AddToRoleAsync(user, ProjectConstant.UserRoles.JobSeeker);
            //    break;
            //}
        }
    }
}