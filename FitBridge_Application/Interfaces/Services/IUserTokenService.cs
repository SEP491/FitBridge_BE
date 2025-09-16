using System;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Interfaces.Services;

public interface IUserTokenService
{
    /// <summary>
        /// Creates an ID token for the specified user with their role.
        /// </summary>
        /// <param name="user">The application user.</param>
        /// <param name="role">The user's role.</param>
        /// <returns>A string representing the ID token.</returns>
        string CreateIdToken(ApplicationUser user, List<string> roles);

        /// <summary>
        /// Creates an access token for the specified user with their role.
        /// </summary>
        /// <param name="user">The application user.</param>
        /// <param name="role">The user's role.</param>
        /// <returns>A string representing the access token.</returns>
        string CreateAccessToken(ApplicationUser user, List<string> roles);

        /// <summary>
        /// Creates a refresh token for the specified user.
        /// </summary>
        /// <param name="user">The application user.</param>
        /// <returns>A string representing the refresh token.</returns>
        string CreateRefreshToken(ApplicationUser user);

        /// <summary>
        /// Revokes refresh token for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user whose tokens should be revoked.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RevokeUserToken(string userId);
}
