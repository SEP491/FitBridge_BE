namespace FitBridge_Application.Commons.Constants;

public static class ProjectConstant
{
    public const string defaultAvatar = "https://img.icons8.com/?size=100&id=tZuAOUGm9AuS&format=png&color=000000";
    public const int MaximumAvatarSize = 4;
    public const string DefaultShopAddressId = "8be321e7-ccb9-467c-884b-af1259ec4aaa";
    public const string DefaultAhamoveServiceId = "SGN-ECO";

    public static class UserRoles
    {
        public const string FreelancePT = "FreelancePT";

        public const string GymPT = "GymPT";

        public const string Admin = "Admin";

        public const string GymOwner = "GymOwner";

        public const string Customer = "Customer";
    }

    public static class FeatureKeyNames
    {
        public const string ChatbotAI = "ChatbotAI";
        public const string HotResearch = "HotResearch";
    }

    public static class SystemConfigurationKeys
    {
        public const string RemindExpiredSubscriptionBeforeDays = "DayToRemindExpireUserSubscription";
        public const string CommissionRate = "CommissionRate";
        public const string GymSlotDuration = "MinimumGymSlotDuration";
        public const string CancelBookingBeforeHours = "CancelBookingBeforeHours";
        public const string ProfitDistributionDays = "ProfitDistributionDays";
        public const string HotResearchSubscriptionLimit = "MaximumHotResearch";
        public const string NearExpiredDateProductWarning = "NearExpiredDateProductWarning";
        public const string AutoHideProductBeforeExpirationDate = "AutoHideProductBeforeExpirationDate";
        public const string AutoFinishArrivedOrderAfterTime = "AutoFinishArrivedOrderAfterTime";
        public const string AutoMarkAsFeedbackAfterDays = "AutoMarkAsFeedbackAfterDays";
        public const string MaximumReviewImages = "MaximumReviewImages";
    }
    public const int MaxRetries = 3;
    public static class EmailTypes
    {
        public const string InformationEmail = "InformationEmail";
        public const string RegistrationConfirmationEmail = "RegistrationConfirmationEmail";
    }
}