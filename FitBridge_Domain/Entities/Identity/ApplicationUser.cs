using FitBridge_Domain.Entities.Accounts;
using FitBridge_Domain.Entities.Blogging;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Entities.QAndA;
using Microsoft.AspNetCore.Identity;
using FitBridge_Domain.Enums.ApplicationUser;
using FitBridge_Domain.Entities.Meetings;

namespace FitBridge_Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; }
        public bool IsMale { get; set; }
        public DateTime Dob { get; set; }
        public string Password { get; set; }
        public string? GymName { get; set; }
        public string TaxCode { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public bool hotResearch { get; set; }
        public string? GymDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public AccountStatus AccountStatus { get; set; }
        public string? RefreshToken { get; set; }
        public string? AvatarUrl { get; set; }
        public Guid? GymOwnerId { get; set; } //To know this gym pt belong to which gym owner
        public int PtMaxCourse { get; set; }
        public int MinimumSlot { get; set; } // Minimum slot register perweek, control by gym owner account
        public ApplicationUser? GymOwner { get; set; }
        public ICollection<ApplicationUser> GymPTs { get; set; } = new List<ApplicationUser>();
        public List<string> GymImages { get; set; } = new List<string>();
        public ICollection<GoalTraining> GoalTrainings { get; set; } = new List<GoalTraining>();
        public ICollection<GymCoursePT> GymCoursePTs { get; set; } = new List<GymCoursePT>();
        public ICollection<GymFacility> GymFacilities { get; set; } = new List<GymFacility>();
        public ICollection<PTGymSlot> PTGymSlots { get; set; } = new List<PTGymSlot>();
        public ICollection<GymSlot> GymSlots { get; set; } = new List<GymSlot>();
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
        public ICollection<ConversationMember> ConversationMembers { get; set; } = new List<ConversationMember>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public UserDetail? UserDetail { get; set; }
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
        public ICollection<GymCourse> GymCourses { get; set; } = new List<GymCourse>();
        public ICollection<WithdrawalRequest> WithdrawalRequests { get; set; } = new List<WithdrawalRequest>();
        public ICollection<PushNotificationTokens> PushNotificationTokens { get; set; } = new List<PushNotificationTokens>();
        public ICollection<Notification> InAppNotifications { get; set; } = new List<Notification>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Review> GymReviews { get; set; } = new List<Review>();
        public ICollection<Review> FreelancePtReviews { get; set; } = new List<Review>();
        public ICollection<FreelancePTPackage> PTFreelancePackages { get; set; } = new List<FreelancePTPackage>();
        public ICollection<CustomerPurchased> CustomerPurchased { get; set; } = new List<CustomerPurchased>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<MeetingSession> MeetingSessions { get; set; } = new List<MeetingSession>();
    }

}