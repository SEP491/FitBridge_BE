using FitBridge_Application.Dtos.Meetings;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Bookings;
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
            var spec = new GetBookingByIdSpec(request.BookingId, isIncludePtGymSlot: true);
            var booking = await unitOfWork.Repository<Booking>().GetBySpecificationAsync(spec)
                    ?? throw new NotFoundException(nameof(Booking));

            ArgumentNullException.ThrowIfNull(booking.PTGymSlot);

            var newMeetingSession = new MeetingSession
            {
                UserOneId = booking.PTGymSlot.PTId,
                UserTwoId = booking.CustomerId,
                BookingId = request.BookingId
            };

            unitOfWork.Repository<MeetingSession>().Insert(newMeetingSession);
            await unitOfWork.CommitAsync();

            return new CreateMeetingDto { MeetingId = newMeetingSession.Id };
        }
    }
}