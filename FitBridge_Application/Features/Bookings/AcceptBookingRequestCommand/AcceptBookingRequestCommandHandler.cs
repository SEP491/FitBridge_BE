using System;
using MediatR;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.Trainings;
using FitBridge_Application.Specifications.Bookings;
using FitBridge_Application.Specifications.Bookings.GetFreelancePtBookingForValidation;
using FitBridge_Domain.Exceptions;
using AutoMapper;

namespace FitBridge_Application.Features.Bookings.AcceptBookingRequestCommand;

public class AcceptBookingRequestCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<AcceptBookingRequestCommand, Guid>
{
    public async Task<Guid> Handle(AcceptBookingRequestCommand request, CancellationToken cancellationToken)
    {
        var bookingRequest = await _unitOfWork.Repository<BookingRequest>().GetByIdAsync(request.BookingRequestId);
        if (bookingRequest == null)
        {
            throw new NotFoundException("Booking request not found");
        }
        if (bookingRequest.RequestType == RequestType.CustomerCreate || bookingRequest.RequestType == RequestType.PtCreate)
        {
            await ValidateBookingRequest(bookingRequest);
            var newBooking = _mapper.Map<Booking>(bookingRequest);
            bookingRequest.RequestStatus = BookingRequestStatus.Approved;
            _unitOfWork.Repository<Booking>().Insert(newBooking);
            _unitOfWork.Repository<BookingRequest>().Update(bookingRequest);
        }
        await _unitOfWork.CommitAsync();
        return request.BookingRequestId;
    }

    public async Task<bool> ValidateBookingRequest(BookingRequest bookingRequest)
    {

        var bookingSpec = new GetBookingForValidationSpec(bookingRequest.CustomerId, bookingRequest.BookingDate, bookingRequest.StartTime, bookingRequest.EndTime);
        var booking = await _unitOfWork.Repository<Booking>().GetBySpecificationAsync(bookingSpec, false);
        if (booking != null)
        {
            throw new DuplicateException($"Customer have course at this time, booking that overlapped: {booking.Id}");
        }
        var freelancePtBookingSpec = new GetFreelancePtBookingForValidationSpec(bookingRequest.PtId, bookingRequest.BookingDate, bookingRequest.StartTime, bookingRequest.EndTime);
        var freelancePtBooking = await _unitOfWork.Repository<Booking>().GetBySpecificationAsync(freelancePtBookingSpec, false);
        if (freelancePtBooking != null)
        {
            throw new DuplicateException($"PT have course at this time booking that overlapped: {freelancePtBooking.Id}");
        }
        return true;
    }
}
