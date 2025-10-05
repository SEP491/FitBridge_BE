using FitBridge_Application.Dtos.Meetings;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Meetings;
using FitBridge_Domain.Entities.Meetings;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Meetings.GetMeeting
{
    internal class GetMeetingQueryHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<GetMeetingQuery, GetMeetingDto>
    {
        public async Task<GetMeetingDto> Handle(GetMeetingQuery request, CancellationToken cancellationToken)
        {
            var spec = new GetMeetingSessionByBookingIdSpecification(request.BookingId);
            var meetingSession = await unitOfWork.Repository<MeetingSession>()
                .GetBySpecificationAsync(spec)
                ?? throw new NotFoundException("Meeting session not found for the given booking ID.");

            return new GetMeetingDto { Id = meetingSession.Id };
        }
    }
}