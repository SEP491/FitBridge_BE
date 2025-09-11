using FitBridge_Application.Commons.Constants;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Infrastructure.Seeder;

internal static class IdentityData
{
    public static IEnumerable<ApplicationUser> GetUsers()
    {
        List<ApplicationUser> users = new()
        {
            new ApplicationUser
            {
            },
        };

        return users;
    }

    public static IEnumerable<ApplicationRole> GetRoles()
    {
        List<ApplicationRole> roles = new()
        {
            new(ProjectConstant.UserRoles.Admin)
            {
                NormalizedName = ProjectConstant.UserRoles.Admin.ToUpper()
            },
            new(ProjectConstant.UserRoles.FreelancePT)
            {
                NormalizedName = ProjectConstant.UserRoles.FreelancePT.ToUpper()
            },
            new(ProjectConstant.UserRoles.GymPT)
            {
                NormalizedName = ProjectConstant.UserRoles.GymPT.ToUpper()
            },
            new(ProjectConstant.UserRoles.GymOwner)
            {
                NormalizedName = ProjectConstant.UserRoles.GymOwner.ToUpper()
            },
            new(ProjectConstant.UserRoles.Customer)
            {
                NormalizedName = ProjectConstant.UserRoles.Customer.ToUpper()
            }
        };

        return roles;
    }
}