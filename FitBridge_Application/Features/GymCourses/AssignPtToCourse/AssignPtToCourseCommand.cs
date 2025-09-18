using MediatR;

namespace FitBridge_Application.Features.GymCourses.AssignPtToCourse
{
    public class AssignPtToCourseCommand : IRequest<Guid>
    {
        public string PtId { get; set; }

        public string GymCourseId { get; set; }

        public int Session { get; set; }
    }
}