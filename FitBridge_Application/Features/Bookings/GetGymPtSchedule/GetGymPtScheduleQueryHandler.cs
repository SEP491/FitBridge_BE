using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Bookings;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Bookings.GetGymPtSchedule;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Bookings.GetGymPtSchedule
{
    internal class GetGymPtScheduleQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserUtil userUtil,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetGymPtScheduleQuery, PagingResultDto<GetGymPtScheduleResponse>>
    {
        public async Task<PagingResultDto<GetGymPtScheduleResponse>> Handle(GetGymPtScheduleQuery request, CancellationToken cancellationToken)
        {
            var userId = userUtil.GetAccountId(httpContextAccessor.HttpContext);
            if (userId == null)
            {
                throw new NotFoundException("Gym pt");
            }

            var spec = new GetGymPtScheduleSpec(request.Params, userId.Value);
            var bookings = await unitOfWork.Repository<Booking>()
             .GetAllWithSpecificationProjectedAsync<GetGymPtScheduleResponse>(spec, mapper.ConfigurationProvider);
            var totalItems = await unitOfWork.Repository<Booking>().CountAsync(spec);

            return new PagingResultDto<GetGymPtScheduleResponse>(totalItems, bookings);
        }
    }
}