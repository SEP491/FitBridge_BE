using FitBridge_Application.Commons.Constants;
using FitBridge_Domain.Entities.Accounts;
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
                Id = Guid.NewGuid(),
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@fitbridge.com",
                NormalizedEmail = "ADMIN@FITBRIDGE.COM",
                EmailConfirmed = true,
                PhoneNumber = "1234567890",
                PhoneNumberConfirmed = true,
                GymImages = [],
                GoalTrainings = [],
                GymCoursePTs = [],
                GymFacilities = [],
                PTGymSlots = [],
                GymSlots = [],
                Addresses = [],
                Blogs = [],
                ConversationMembers = [],
                Bookings = [],
                Orders = [],
                UserDetail = new UserDetail(),
                Questions = [],
                Coupons = [],
                GymCourses = [],
                WithdrawalRequests = [],
                PushNotificationTokens = [],
                InAppNotifications = [],
                Reviews = [],
                GymReviews = [],
                FreelancePtReviews = [],
                PTFreelancePackages = []
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