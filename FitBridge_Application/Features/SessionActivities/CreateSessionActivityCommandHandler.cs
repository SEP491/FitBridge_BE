using System;
using FitBridge_Application.Dtos.SessionActivities;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Trainings;
using AutoMapper;
using MediatR;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.Trainings;

namespace FitBridge_Application.Features.SessionActivities;

public class CreateSessionActivityCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<CreateSessionActivityCommand, SessionActivitiyResponseDto>
{
    public async Task<SessionActivitiyResponseDto> Handle(CreateSessionActivityCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(request.BookingId);
        if (booking == null)
        {
            throw new NotFoundException("Booking id not found");
        }
        var mappedEntity = _mapper.Map<CreateSessionActivityCommand, SessionActivity>(request);
        mappedEntity.BookingId = request.BookingId;
        _unitOfWork.Repository<SessionActivity>().Insert(mappedEntity);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<SessionActivity, SessionActivitiyResponseDto>(mappedEntity);
    }
}
