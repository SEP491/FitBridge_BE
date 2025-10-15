using FitBridge_Domain.Entities.ServicePackages;

namespace FitBridge_Application.Specifications.Memberships.GetAllMemberships
{
    public class GetAllMembershipsSpecification : BaseSpecification<ServiceInformation>
    {
        public GetAllMembershipsSpecification(GetAllMembershipsParam param,
            bool isIncludeOrderItems = false) : base(x =>
            x.IsEnabled)
        {
            if (param.DoApplyPaging)
            {
                AddPaging(param.Size * (param.Page - 1), param.Size);
            }
            if (isIncludeOrderItems)
            {
                AddInclude(x => x.OrderItems);
            }
        }
    }
}