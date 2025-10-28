using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Application.Dtos.Bookings
{
    public class GetBookingHistoryResponseDto
{
        public Guid Id { get; set; }

  public DateOnly BookingDate { get; set; }

public TimeOnly? StartTime { get; set; }

   public TimeOnly? EndTime { get; set; }

  public string? BookingName { get; set; }

      public SessionStatus SessionStatus { get; set; }

   public string? Note { get; set; }

        public string? NutritionTip { get; set; }

   public DateTime? SessionStartTime { get; set; }

  public DateTime? SessionEndTime { get; set; }

// Customer Information (for PT views)
        public Guid? CustomerId { get; set; }

 public string? CustomerName { get; set; }

    public string? CustomerAvatarUrl { get; set; }

        // PT Information (for customer views)
        public Guid? PtId { get; set; }

   public string? PtName { get; set; }

        public string? PtAvatarUrl { get; set; }

   // Gym Slot Information (for gym-based bookings)
   public Guid? PTGymSlotId { get; set; }

        public string? GymSlotName { get; set; }

        // Package Information (for freelance PT bookings)
   public string? PackageName { get; set; }

        public DateTime CreatedAt { get; set; }

   public DateTime UpdatedAt { get; set; }
    }
}
