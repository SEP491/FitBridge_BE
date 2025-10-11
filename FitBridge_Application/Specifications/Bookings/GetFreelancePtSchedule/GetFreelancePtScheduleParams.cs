using System;

namespace FitBridge_Application.Specifications.Bookings.GetFreelancePtSchedule;

public class GetFreelancePtScheduleParams : BaseParams
{
    public DateOnly Date { get; set; }
}
