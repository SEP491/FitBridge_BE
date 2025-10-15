using FitBridge_Domain.Entities.Trainings;

namespace FitBridge_Application.Specifications.Bookings.GetBookingById
{
    public class GetBookingByIdSpec : BaseSpecification<Booking>
    {
        public GetBookingByIdSpec(Guid bookingId, bool isIncludePtGymSlot = false) : base(x =>
            x.IsEnabled && x.Id == bookingId)
        {
            if (isIncludePtGymSlot)
            {
                AddInclude(x => x.PTGymSlot!);
            }
        }
    }
}