using MediatR;
using System;

namespace FitBridge_Application.Features.GymCourses.DeleteGymCourseById
{
    public class DeleteGymCourseByIdCommand(Guid gymCourseId) : IRequest
    {
        public Guid GymCourseId { get; set; } = gymCourseId;
    }
}