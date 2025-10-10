namespace FitBridge_Domain.Enums.MessageAndReview
{
    public enum EnumContentType
    {
        NewMessage,

        TrainingSlotCancelled,

        IncomingTrainingSlot,

        NewGymFeedback,

        PaymentRequest,

        PackageBought,

        CreateBookingRequest, // Customer/FreelancePT create a booking request to create a booking

        EditBookingRequest, // Customer/FreelancePT create a request to edit a booking

        RejectBookingRequest, // Customer/FreelancePT reject a booking request

        AcceptBookingRequest, // Customer/FreelancePT accept a booking request
    }
}