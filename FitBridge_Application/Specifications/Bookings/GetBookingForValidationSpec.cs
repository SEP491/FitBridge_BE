using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Application.Specifications.Bookings;

public class GetBookingForValidationSpec : BaseSpecification<Booking>
{
    public GetBookingForValidationSpec(Guid customerId, DateOnly bookingDate, TimeOnly startTime, TimeOnly endTime)
        : base(x => x.CustomerId == customerId
            && x.BookingDate == bookingDate
            && x.IsEnabled
            && x.SessionStatus == SessionStatus.Booked
            && (
                // Freelance PT time overlap
                (x.PtFreelanceStartTime.HasValue && x.PtFreelanceEndTime.HasValue
                 && !(startTime >= x.PtFreelanceEndTime.Value || endTime <= x.PtFreelanceStartTime.Value))
                ||
                // Gym slot time overlap
                (x.PTGymSlot != null && x.PTGymSlot.GymSlot != null
                 && !(startTime >= x.PTGymSlot.GymSlot.EndTime || endTime <= x.PTGymSlot.GymSlot.StartTime))
            ))
    {
    }
}
