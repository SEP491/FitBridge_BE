using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Specifications.CustomerPurchaseds.GetFreelancePtCustomerPurchased
{
    public class GetFreelancePtCustomerPurchasedSpec : BaseSpecification<CustomerPurchased>
    {
        public GetFreelancePtCustomerPurchasedSpec(Guid ptId, GetFreelancePtCustomerPurchasedParams parameters) : base(x =>
            x.IsEnabled &&
            x.OrderItems.Any(oi => oi.FreelancePTPackageId != null && oi.FreelancePTPackageId == ptId) &&
            (string.IsNullOrEmpty(parameters.SearchTerm) ||
            x.Customer.FullName.ToLower().Contains(parameters.SearchTerm.ToLower()) ||
            x.Customer.Email.ToLower().Contains(parameters.SearchTerm.ToLower())))
        {
            AddInclude(x => x.Customer);

            AddInclude(x => x.OrderItems);
            AddInclude("OrderItems.FreelancePTPackage");
            AddOrderByDesc(x => x.ExpirationDate);

            if (parameters.DoApplyPaging)
            {
                AddPaging(parameters.Size * (parameters.Page - 1), parameters.Size);
            }
        }
    }
}