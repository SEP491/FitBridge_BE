using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Application.Specifications.Bookings.GetFreelancePtBookingForValidation;

public class GetFreelancePtBookingForValidationSpec : BaseSpecification<Booking>
{
    public GetFreelancePtBookingForValidationSpec(Guid ptId, DateOnly bookingDate, TimeOnly ptFreelanceStartTime, TimeOnly ptFreelanceEndTime) : base(x =>
    x.PtId == ptId
    && x.BookingDate == bookingDate
    && x.IsEnabled
    && x.SessionStatus == SessionStatus.Booked
    && !(x.PtFreelanceStartTime >= ptFreelanceEndTime || x.PtFreelanceEndTime <= ptFreelanceStartTime))
    {
    }
}
