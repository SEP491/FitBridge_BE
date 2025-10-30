using System;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Domain.Entities.Accounts;

public class UserDetail : BaseEntity
{
    public double? Biceps { get; set; }
    public double? ForeArm { get; set; }
    public double? Thigh { get; set; }
    public double? Calf { get; set; }
    public double? Chest { get; set; }
    public double? Waist { get; set; }
    public double? Hip { get; set; }
    public double? Shoulder { get; set; }
    public double? Height { get; set; }
    public double? Weight { get; set; }
    public List<string>? Certificates { get; set; }
    public int? Experience { get; set; }
    public string? Bio { get; set; }

    public ApplicationUser User { get; set; }
}
