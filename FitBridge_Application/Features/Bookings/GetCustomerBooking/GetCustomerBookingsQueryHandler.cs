using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Specifications.Bookings.GetCustomerBookings;
using AutoMapper;
using MediatR;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Application.Dtos.Bookings;

namespace FitBridge_Application.Features.Bookings.GetCustomerBooking;

public class GetCustomerBookingsQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IApplicationUserService _applicationUserService) : IRequestHandler<GetCustomerBookingsQuery, PagingResultDto<GetCustomerBookingsResponse>>
{
    public async Task<PagingResultDto<GetCustomerBookingsResponse>> Handle(GetCustomerBookingsQuery request, CancellationToken cancellationToken)
    {
        var customerEntity = await _applicationUserService.GetByIdAsync(request.Params.CustomerId);
        if (customerEntity == null)
        {
            throw new NotFoundException("Customer not found");
        }
        var bookings = await _unitOfWork.Repository<Booking>().GetAllWithSpecificationProjectedAsync<GetCustomerBookingsResponse>(new GetCustomerBookingByCustomerIdSpecification(request.Params), _mapper.ConfigurationProvider);
        var totalItems = await _unitOfWork.Repository<Booking>().CountAsync(new GetCustomerBookingByCustomerIdSpecification(request.Params));

        return new PagingResultDto<GetCustomerBookingsResponse>(totalItems, bookings);
    }

}
