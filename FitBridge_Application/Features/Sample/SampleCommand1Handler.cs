using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FitBridge_Application.Features.Sample
{
    internal class SampleCommand1Handler : IRequestHandler<SampleCommand1>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public SampleCommand1Handler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task Handle(SampleCommand1 request, CancellationToken cancellationToken)
        {
            var user = new IdentityUser
            {
                UserName = request.UserName,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                // Handle creation failure
                throw new InvalidOperationException($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}