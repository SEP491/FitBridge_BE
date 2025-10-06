namespace FitBridge_Application.Commons.Constants;

public static class ProjectConstant
{
    public const string defaultAvatar = "https://img.icons8.com/?size=100&id=tZuAOUGm9AuS&format=png&color=000000";

    public static class UserRoles
    {
        public const string FreelancePT = "FreelancePT";

        public const string GymPT = "GymPT";

        public const string Admin = "Admin";

        public const string GymOwner = "GymOwner";

        public const string Customer = "Customer";
    }
    public const int CancelBookingBeforeHours = 4;

    public const int GymSlotDuration = 1;

    public const decimal CommissionRate = 0.1m;
}