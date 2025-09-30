using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.Gyms;

namespace FitBridge_Domain.Entities.Gyms;

public class PTGymSlot : BaseEntity
{
    public Guid PTId { get; set; }

    public Guid GymSlotId { get; set; }

    public PTGymSlotStatus Status { get; set; }
    public DateOnly RegisterDate { get; set; }

    public ApplicationUser PT { get; set; }

    public GymSlot GymSlot { get; set; }

    public Booking Booking { get; set; }
}