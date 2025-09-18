using AutoMapper;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Gyms;
using MediatR;

namespace FitBridge_Application.Features.GymCourses.AssignPtToCourse
{
    internal class AssignPtToCourseCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<AssignPtToCourseCommand, Guid>
    {
        public async Task<Guid> Handle(AssignPtToCourseCommand request, CancellationToken cancellationToken)
        {
            var mappedEntity = mapper.Map<AssignPtToCourseCommand, GymCoursePT>(request);
            var newId = Guid.NewGuid();
            mappedEntity.Id = newId;
            unitOfWork.Repository<GymCoursePT>().Insert(mappedEntity);

            await unitOfWork.CommitAsync();

            return newId;
        }
    }
}