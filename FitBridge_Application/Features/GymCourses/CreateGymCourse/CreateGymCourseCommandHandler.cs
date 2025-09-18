using AutoMapper;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Gyms;
using MediatR;

namespace FitBridge_Application.Features.GymCourses.CreateGymCourse
{
    internal class CreateGymCourseCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<CreateGymCourseCommand, Guid>
    {
        public async Task<Guid> Handle(CreateGymCourseCommand request, CancellationToken cancellationToken)
        {
            var mappedEntity = mapper.Map<CreateGymCourseCommand, GymCourse>(request);
            var newId = Guid.NewGuid();
            mappedEntity.Id = newId;
            mappedEntity.GymOwnerId = Guid.Parse(request.GymOwnerId);

            unitOfWork.Repository<GymCourse>().Insert(mappedEntity);
            await unitOfWork.CommitAsync();
            return newId;
        }
    }
}