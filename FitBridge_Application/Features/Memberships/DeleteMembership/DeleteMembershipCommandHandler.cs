using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Memberships.GetMembershipById;
using FitBridge_Domain.Entities.ServicePackages;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Memberships.DeleteMembership
{
    internal class DeleteMembershipCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<DeleteMembershipCommand>
    {
        public async Task Handle(DeleteMembershipCommand request, CancellationToken cancellationToken)
        {
            var spec = new GetMembershipByIdSpecification(request.MembershipId);
            var membership = await unitOfWork.Repository<ServiceInformation>()
                .GetBySpecificationAsync(spec, asNoTracking: false)
                ?? throw new NotFoundException("Membership not found");

            unitOfWork.Repository<ServiceInformation>().SoftDelete(membership);
            await unitOfWork.CommitAsync();
        }
    }
}