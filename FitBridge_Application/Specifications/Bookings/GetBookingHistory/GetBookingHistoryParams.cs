using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Application.Specifications.Bookings.GetBookingHistory
{
public class GetBookingHistoryParams : BaseParams
    {
        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

   public SessionStatus? Status { get; set; }
    }
}
