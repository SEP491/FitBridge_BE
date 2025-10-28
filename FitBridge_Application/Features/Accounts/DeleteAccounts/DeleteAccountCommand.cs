using MediatR;

namespace FitBridge_Application.Features.Accounts.DeleteAccounts
{
    public class DeleteAccountCommand : IRequest
    {
        public List<Guid> UserIdDeleteList { get; set; } = new List<Guid>();
    }
}
