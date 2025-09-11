using System;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Domain.Entities.Gyms;

public class GymCoursePT : BaseEntity
{
    public Guid GymCourseId { get; set; }
    public Guid PTId { get; set; }

    public GymCourse GymCourse { get; set; }
    public ApplicationUser PT { get; set; }
    public int? Session { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
