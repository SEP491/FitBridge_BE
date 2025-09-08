using MediatR;

namespace FitBridge_Application.Features.Sample
{
    public class SampleCommand1 : IRequest
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}