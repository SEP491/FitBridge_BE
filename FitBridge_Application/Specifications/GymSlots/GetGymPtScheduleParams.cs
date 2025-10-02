using System;
using System.ComponentModel.DataAnnotations;

namespace FitBridge_Application.Specifications.GymSlots;

public class GetGymPtScheduleParams : BaseParams
{
    [Required]
    public Guid PtId { get; set; }
    [Required]
    public DateOnly Date { get; set; }
}
