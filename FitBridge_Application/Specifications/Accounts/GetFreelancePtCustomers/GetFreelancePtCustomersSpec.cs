using FitBridge_Application.Specifications.CustomerPurchaseds.GetFreelancePtCustomerPurchased;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Specifications.Accounts.GetFreelancePtCustomers
{
    public class GetFreelancePtCustomersSpec : BaseSpecification<ApplicationUser>
    {
        public GetFreelancePtCustomersSpec(Guid freelancePtId, GetFreelancePtCustomerParams parameters) : base(x =>
            x.CustomerPurchased.Any(cp => cp.OrderItems.Any(oi => oi.FreelancePTPackageId != null
                                                                && oi.FreelancePTPackage!.PtId == freelancePtId))
            && (string.IsNullOrEmpty(parameters.SearchTerm) ||
            x.FullName.ToLower().Contains(parameters.SearchTerm.ToLower()) ||
            x.Email.ToLower().Contains(parameters.SearchTerm.ToLower()))
        )
        {
            if (parameters.DoApplyPaging)
            {
                AddPaging(parameters.Size * (parameters.Page - 1), parameters.Size);
            }
        }
    }
}