using FitBridge_Application.Dtos.Meetings;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Bookings;
using FitBridge_Application.Specifications.Bookings.GetBookingById;
using FitBridge_Application.Specifications.Meetings;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Meetings;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Meetings.CreateMeeting
{
    internal class CreateMeetingCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<CreateMeetingCommand, CreateMeetingDto>
    {
        async Task<CreateMeetingDto> IRequestHandler<CreateMeetingCommand, CreateMeetingDto>.Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
        {
            var meetingSpec = new GetMeetingSessionByBookingIdSpecification(request.BookingId);
            var existingMeeting = await unitOfWork.Repository<MeetingSession>().GetBySpecificationAsync(
                meetingSpec);

            if (existingMeeting != null)
            {
                throw new DuplicateException("Meeting session already exists for this booking.");
            }

            var spec = new GetBookingByIdSpec(request.BookingId);
            var booking = await unitOfWork.Repository<Booking>().GetBySpecificationAsync(
                spec,
                asNoTracking: false)
                    ?? throw new NotFoundException(nameof(Booking));

            var newMeetingSession = new MeetingSession
            {
                Id = Guid.NewGuid(),
                UserOneId = booking.PtId!.Value,
                UserTwoId = booking.CustomerId,
                BookingId = request.BookingId
            };

            unitOfWork.Repository<MeetingSession>().Insert(newMeetingSession);
            booking.MeetingSession = newMeetingSession;

            unitOfWork.Repository<Booking>().Update(booking);
            await unitOfWork.CommitAsync();

            return new CreateMeetingDto { Id = newMeetingSession.Id };
        }
    }
}