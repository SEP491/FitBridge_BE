using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Enums.GymCourses;

namespace FitBridge_Domain.Entities.Gyms;

public class GymCourse : BaseEntity
{
    public string Name { get; set; }

    public decimal Price { get; set; }

    public int Duration { get; set; }

    public TypeCourseEnum Type { get; set; }

    public string Description { get; set; }

    public string ImageUrl { get; set; }

    public Guid GymOwnerId { get; set; }

    public ICollection<GymCoursePT> GymCoursePTs { get; set; } = new List<GymCoursePT>();

    public ApplicationUser GymOwner { get; set; }
}