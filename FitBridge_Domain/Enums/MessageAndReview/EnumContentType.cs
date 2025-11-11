namespace FitBridge_Domain.Enums.MessageAndReview
{
    public enum EnumContentType
    {
        NewMessage,

        TrainingSlotCancelled,

        IncomingTrainingSlot,

        NewGymFeedback,

        PackageBought,

        NewCoupon,

        RefundedItem, // OrderItem refunded

        // Withdrawal request
        NewPaymentRequest,

        WithdrawalRequestAdminApproved,

        WithdrawalRequestAdminRejected,

        WithdrawalRequestUserDisapproved,

        // Bookings

        CreateBookingRequest, // Customer/FreelancePT create a booking request to create a booking

        EditBookingRequest, // Customer/FreelancePT create a request to edit a booking

        RejectBookingRequest, // Customer/FreelancePT reject a booking request

        AcceptBookingRequest, // Customer/FreelancePT accept a booking request

        // Reports
        NewReport, // Customer creates a new report case

        ReportStatusUpdated, // Admin updates report status
        NearExpiredSubscriptionReminder, // Customer's subscription is about to expire
    }
}