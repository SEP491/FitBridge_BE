using FitBridge_Domain.Entities.Accounts;
using FitBridge_Domain.Entities.Blogging;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Entities.QAndA;
using Microsoft.AspNetCore.Identity;

namespace FitBridge_Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; }
        public bool IsMale { get; set; }
        public DateTime Dob { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string GymName { get; set; }
        public string TaxCode { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public bool hotResearch { get; set; }
        public string? GymDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public AccountStatus AccountStatus { get; set; }
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
        public UserDetail UserDetail { get; set; } = new UserDetail();
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
        public ICollection<GymCourse> GymCourses { get; set; } = new List<GymCourse>();
        public ICollection<WithdrawalRequest> WithdrawalRequests { get; set; } = new List<WithdrawalRequest>();
        public ICollection<PushNotificationTokens> PushNotificationTokens { get; set; } = new List<PushNotificationTokens>();
        public ICollection<InAppNotification> InAppNotifications { get; set; } = new List<InAppNotification>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Review> GymReviews { get; set; } = new List<Review>();
        public ICollection<Review> FreelancePtReviews { get; set; } = new List<Review>();
        public ICollection<FreelancePTPackage> PTFreelancePackages { get; set; } = new List<FreelancePTPackage>();
    }

    public enum AccountStatus
    {
        Active,
        Inactive,
        Deleted
    }
}