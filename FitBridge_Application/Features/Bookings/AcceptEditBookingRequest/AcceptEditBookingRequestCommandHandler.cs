using System;
using FitBridge_Application.Dtos.Bookings;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using FitBridge_Domain.Exceptions;
using AutoMapper;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.Trainings;
using FitBridge_Application.Specifications.Bookings;
using FitBridge_Application.Specifications.Bookings.GetFreelancePtBookingForValidate;

namespace FitBridge_Application.Features.Bookings.AcceptEditBookingRequest;

public class AcceptEditBookingRequestCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<AcceptEditBookingRequestCommand, UpdateBookingResponseDto>
{
    public async Task<UpdateBookingResponseDto> Handle(AcceptEditBookingRequestCommand request, CancellationToken cancellationToken)
    {
        var bookingRequest = await _unitOfWork.Repository<BookingRequest>().GetByIdAsync(request.BookingRequestId);
        if (bookingRequest == null)
        {
            throw new NotFoundException("Booking request not found");
        }
        if (bookingRequest.RequestType != RequestType.PtUpdate
        && bookingRequest.RequestType != RequestType.CustomerUpdate)
        {
            throw new BusinessException("Booking request is not pt update or customer update");
        }
        if (bookingRequest.RequestStatus != BookingRequestStatus.Pending)
        {
            throw new BusinessException("Booking request is not pending");
        }
        if (bookingRequest.TargetBookingId == null)
        {
            throw new NotFoundException("Target booking id not found");
        }
        await ValidateBookingRequest(bookingRequest, bookingRequest.CustomerId, bookingRequest.PtId);
        var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(bookingRequest.TargetBookingId.Value);
        if (booking == null)
        {
            throw new NotFoundException("Booking not found");
        }
        booking.BookingDate = bookingRequest.BookingDate;
        booking.PtFreelanceStartTime = bookingRequest.StartTime;
        booking.PtFreelanceEndTime = bookingRequest.EndTime;
        booking.BookingName = bookingRequest.BookingName;
        booking.Note = bookingRequest.Note;
        bookingRequest.RequestStatus = BookingRequestStatus.Approved;
        _unitOfWork.Repository<Booking>().Update(booking);
        _unitOfWork.Repository<BookingRequest>().Update(bookingRequest);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<UpdateBookingResponseDto>(booking);
    }
    
    public async Task<bool> ValidateBookingRequest(BookingRequest request, Guid customerId, Guid ptId)
    {
        var bookingSpec = new GetBookingForValidationSpec(customerId, request.BookingDate, request.StartTime, request.EndTime);
        var booking = await _unitOfWork.Repository<Booking>().GetAllWithSpecificationAsync(bookingSpec);
        if (booking.Count > 0 )
        {
            if(booking.Count == 1 && booking.First().Id == request.TargetBookingId)
            {
            } else {
                throw new DuplicateException($"Customer have course at this time, booking that overlapped: {booking.First().Id}");
            }
        }
        var freelancePtBookingSpec = new GetFreelancePtBookingForValidationSpec(ptId, request.BookingDate, request.StartTime, request.EndTime);
        var freelancePtBooking = await _unitOfWork.Repository<Booking>().GetAllWithSpecificationAsync(freelancePtBookingSpec);
        if (freelancePtBooking.Count > 0 )
        {
            if(freelancePtBooking.Count == 1 && freelancePtBooking.First().Id == request.TargetBookingId)
            {
            } else {
                throw new DuplicateException($"Customer have course at this time, booking that overlapped: {freelancePtBooking.First().Id}");
            }
        }
        return true;
    }

}
