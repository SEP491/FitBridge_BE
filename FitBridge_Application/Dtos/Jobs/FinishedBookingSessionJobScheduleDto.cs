using System;

namespace FitBridge_Application.Dtos.Jobs;

public class FinishedBookingSessionJobScheduleDto
{
    public Guid BookingId { get; set; }
    public DateTime TriggerTime { get; set; }
}
