using FitBridge_Application.Dtos.Tokens;
using FitBridge_Application.Interfaces.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitBridge_Application.Features.Identities.Token
{
    public class RefreshTokenCommandHandler(IUserTokenService userTokenService, IApplicationUserService applicationUserService) : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
    {

        public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = await userTokenService.ValidateRefreshToken(request.RefreshToken);
                if(userId == null)
                {
                    throw new UnauthorizedAccessException("Invalid refresh token");
                }

                var user = await applicationUserService.GetByIdAsync(Guid.Parse(userId));

                if(user == null)
                {
                    throw new UnauthorizedAccessException("User not found");
                }

                var userRoles = await applicationUserService.GetUserRolesAsync(user);

                var newAccessToken = userTokenService.CreateAccessToken(user, userRoles);
                var newIdToken = userTokenService.CreateIdToken(user, userRoles);
                var newRefreshToken = userTokenService.CreateRefreshToken(user);

                user.RefreshToken = newRefreshToken;

                return new RefreshTokenResponse
                {
                    AccessToken = newAccessToken,
                    IdToken = newIdToken,
                    RefreshToken = newRefreshToken
                };
            } catch (Exception e)
            {
                throw new UnauthorizedAccessException("Could not refresh token: ", e);
            }
        }
    }
}
