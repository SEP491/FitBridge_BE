using AutoMapper;
using FitBridge_Application.Dtos.GymCourses;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.GymCourses.GetGymCourseById;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.GymCourses.UpdateGymCourse
{
    internal class UpdateGymCourseCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<UpdateGymCourseCommand, UpdateGymCourseResponse>
    {
        public async Task<UpdateGymCourseResponse> Handle(UpdateGymCourseCommand request, CancellationToken cancellationToken)
        {
            var spec = new GetGymCourseByIdSpecification(request.GymCourseId, includeGymOwner: false);
            var gymCourse = await unitOfWork.Repository<GymCourse>().GetBySpecificationAsync(
                spec,
                asNoTracking: true) ?? throw new NotFoundException(nameof(GymCourse));

            gymCourse.Name = request.Name ?? gymCourse.Name;
            gymCourse.Price = request.Price ?? gymCourse.Price;
            gymCourse.Duration = request.Duration ?? gymCourse.Duration;
            gymCourse.Description = request.Description ?? gymCourse.Description;

            unitOfWork.Repository<GymCourse>().Update(gymCourse);
            await unitOfWork.CommitAsync();

            return new UpdateGymCourseResponse
            {
                Name = gymCourse.Name,
                Price = gymCourse.Price,
                Duration = gymCourse.Duration,
                Type = gymCourse.Type,
                Description = gymCourse.Description,
                ImageUrl = gymCourse.ImageUrl,
                GymOwnerId = gymCourse.GymOwnerId
            };
        }
    }
}