using System;
using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Application.Dtos.Bookings
{
    public class GetGymPtScheduleResponse
    {
        public Guid BookingId { get; set; }

        public string? BookingName { get; set; }

        public DateOnly BookingDate { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public Guid CustomerId { get; set; }

        public Guid CustomerPurchasedId { get; set; }

        public SessionStatus SessionStatus { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerAvatarUrl { get; set; }

        public string? CourseName { get; set; }

        public Guid? PTGymSlotId { get; set; }

        public string? GymSlotName { get; set; }

        public string? Note { get; set; }

        public string? NutritionTip { get; set; }
    }
}