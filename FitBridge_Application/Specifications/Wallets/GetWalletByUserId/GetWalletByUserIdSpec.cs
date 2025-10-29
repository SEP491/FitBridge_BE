using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.Wallets.GetWalletByUserId
{
    public class GetWalletByUserIdSpec : BaseSpecification<Wallet>
    {
        public GetWalletByUserIdSpec(Guid userId) : base(x => x.IsEnabled && x.User.Id == userId)
        {
        }
    }
}