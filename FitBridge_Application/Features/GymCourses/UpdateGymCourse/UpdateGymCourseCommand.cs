using FitBridge_Application.Dtos.GymCourses;
using MediatR;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.GymCourses.UpdateGymCourse
{
    public class UpdateGymCourseCommand : IRequest<UpdateGymCourseResponse>
    {
        [JsonIgnore]
        public Guid GymCourseId { get; set; }

        public string? Name { get; set; }

        public decimal? Price { get; set; }

        public int? Duration { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
    }
}