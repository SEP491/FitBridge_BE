using MediatR;

namespace FitBridge_Application.Features.Accounts.BanUnbanAccounts
{
    public class BanUnbanAccountCommand : IRequest
    {
        public List<Guid> UserIdBanUnbanList { get; set; }

        public bool IsBan { get; set; }
    }
}