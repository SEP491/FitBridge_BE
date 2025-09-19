using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.GymCourses.GetGymCoursesByGymId;
using FitBridge_Domain.Entities.Gyms;
using MediatR;

namespace FitBridge_Application.Features.GymCourses.GetGymCoursesByGymId
{
    internal class GetGymCoursesByGymIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<GetGymCoursesByGymIdQuery, PagingResultDto<GetGymCourseDto>>
    {
        public async Task<PagingResultDto<GetGymCourseDto>> Handle(GetGymCoursesByGymIdQuery request, CancellationToken cancellationToken)
        {
            var spec = new GetGymCoursesByGymIdSpecification(request.GymId, request.GetGymCoursesByGymIdParams);
            var results = await unitOfWork.Repository<GymCourse>().GetAllWithSpecificationProjectedAsync<GetGymCourseDto>(spec, mapper.ConfigurationProvider);

            var totalItems = await unitOfWork.Repository<GymCourse>().CountAsync(spec);

            return new PagingResultDto<GetGymCourseDto>(totalItems, results);
        }
    }
}