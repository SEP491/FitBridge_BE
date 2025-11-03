namespace FitBridge_Application.Commons.Constants;

public static class ProjectConstant
{
    public const string defaultAvatar = "https://img.icons8.com/?size=100&id=tZuAOUGm9AuS&format=png&color=000000";
    public const string ProfitDistributionDays = "ProfitDistributionDays";
    public const int MaximumAvatarSize = 4;

    public static class UserRoles
    {
        public const string FreelancePT = "FreelancePT";

        public const string GymPT = "GymPT";

        public const string Admin = "Admin";

        public const string GymOwner = "GymOwner";

        public const string Customer = "Customer";
    }
    public const string CancelBookingBeforeHours = "CancelBookingBeforeHours";

    public const string GymSlotDuration = "MinimumGymSlotDuration";

    public const string CommissionRate = "CommissionRate";
    public const int MaxRetries = 3;
    public static class EmailTypes
    {
        public const string InformationEmail = "InformationEmail";
        public const string RegistrationConfirmationEmail = "RegistrationConfirmationEmail";
    }
}