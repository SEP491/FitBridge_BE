using FitBridge_Application.Dtos.GymCourses;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Enums.GymCourses;
using MediatR;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.GymCourses.CreateGymCourse
{
    public class CreateGymCourseCommand : IRequest<CreateGymCourseResponse>
    {
        [JsonIgnore]
        public string? GymOwnerId { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Duration { get; set; }

        public TypeCourseEnum Type { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }
    }
}