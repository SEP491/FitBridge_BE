using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.Trainings;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Bookings.GetBookingByCustomerPurchaseId
{
    public class GetBookingByCustomerPurchaseIdSpec : BaseSpecification<Booking>
    {
        public GetBookingByCustomerPurchaseIdSpec(Guid customerPurchasedId) : base(x =>
            x.IsEnabled
            && x.SessionStatus == SessionStatus.Cancelled
            && x.CustomerPurchasedId == customerPurchasedId)
        {
            AddInclude($"{nameof(Booking.SessionActivities)}");
            AddInclude($"{nameof(Booking.SessionActivities)}.{nameof(SessionActivity.ActivitySets)}");
        }
    }
}