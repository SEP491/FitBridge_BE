using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Bookings;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Bookings.GetBookingHistory;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Bookings.GetBookingHistory
{
    public class GetBookingHistoryQueryHandler(
            IUnitOfWork unitOfWork,
       IMapper mapper,
            IUserUtil userUtil,
            IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetBookingHistoryQuery, PagingResultDto<GetBookingHistoryResponseDto>>
    {
        public async Task<PagingResultDto<GetBookingHistoryResponseDto>> Handle(
         GetBookingHistoryQuery request,
             CancellationToken cancellationToken)
        {
            var userId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
        ?? throw new NotFoundException(nameof(ApplicationUser));

            var userRole = userUtil.GetUserRole(httpContextAccessor.HttpContext)
              ?? throw new NotFoundException("User role not found");

            var spec = new GetBookingHistorySpec(request.Params, userId, userRole);

            var bookings = await unitOfWork.Repository<Booking>()
            .GetAllWithSpecificationProjectedAsync<GetBookingHistoryResponseDto>(spec, mapper.ConfigurationProvider);

            var totalCount = await unitOfWork.Repository<Booking>()
            .CountAsync(spec);

            return new PagingResultDto<GetBookingHistoryResponseDto>(totalCount, bookings);
        }
    }
}