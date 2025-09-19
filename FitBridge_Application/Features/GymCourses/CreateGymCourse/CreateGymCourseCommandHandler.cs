using AutoMapper;
using FitBridge_Application.Dtos.GymCourses;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Gyms;
using MediatR;

namespace FitBridge_Application.Features.GymCourses.CreateGymCourse
{
    internal class CreateGymCourseCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<CreateGymCourseCommand, CreateGymCourseResponse>
    {
        public async Task<CreateGymCourseResponse> Handle(CreateGymCourseCommand request, CancellationToken cancellationToken)
        {
            var mappedEntity = mapper.Map<CreateGymCourseCommand, GymCourse>(request);
            var newId = Guid.NewGuid();
            mappedEntity.Id = newId;
            mappedEntity.GymOwnerId = Guid.Parse(request.GymOwnerId);

            unitOfWork.Repository<GymCourse>().Insert(mappedEntity);
            await unitOfWork.CommitAsync();
            return mappedEntity is not null
                ? new CreateGymCourseResponse
                {
                    GymOwnerId = mappedEntity.GymOwnerId,
                    Name = mappedEntity.Name,
                    Description = mappedEntity.Description,
                    Price = mappedEntity.Price,
                    Duration = mappedEntity.Duration,
                    Type = mappedEntity.Type,
                    ImageUrl = mappedEntity.ImageUrl
                }
                : null!;
        }
    }
}