using AutoMapper;
using FitBridge_Application.Interfaces.Specifications;
using FitBridge_Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace FitBridge_Application.Interfaces.Services
{
    public interface IApplicationUserService
    {
        #region Core methods

        /// <summary>
        /// Retrieves all users asynchronously.
        /// </summary>
        /// <returns>A read-only list of all users.</returns>
        Task<IReadOnlyList<ApplicationUser>> GetAllUsersAsync();

        /// <summary>
        /// Retrieves users by their role asynchronously.
        /// </summary>
        /// <param name="role">The role of the users to retrieve.</param>
        /// <returns>A list of users with the specified role.</returns>
        Task<IList<ApplicationUser>> GetUsersByRoleAsync(string role);

        /// <summary>
        /// Retrieves all users that match a given specification asynchronously.
        /// </summary>
        /// <param name="spec">The specification to filter users.</param>
        /// <returns>A read-only list of users that match the specification.</returns>
        Task<IReadOnlyList<ApplicationUser>> GetAllUsersWithSpecAsync(ISpecification<ApplicationUser> spec, bool asNoTracking = true);

        /// <summary>
        /// Retrieves a single user that matches a given specification asynchronously.
        /// </summary>
        /// <param name="spec">The specification to filter the user.</param>
        /// <returns>The user that matches the specification, or null if no user matches.</returns>
        Task<ApplicationUser?> GetUserWithSpecAsync(ISpecification<ApplicationUser> spec, bool asNoTracking = true);

        /// <summary>
        /// Retrieves a single user that matches a given specification asynchronously.
        /// </summary>
        /// <param name="spec">The specification to filter the user.</param>
        /// <returns>The user that matches the specification, or null if no user matches.</returns>
        Task<TDto?> GetUserWithSpecProjectedAsync<TDto>(ISpecification<ApplicationUser> spec, IConfigurationProvider mapperConfig, bool asNoTracking = true);

        /// <summary>
        /// Retrieves a single user that matches a given specification asynchronously.
        /// </summary>
        /// <param name="spec">The specification to filter the user.</param>
        /// <returns>The user that matches the specification, or null if no user matches.</returns>
        Task<List<TDto>> GetAllUserWithSpecProjectedAsync<TDto>(ISpecification<ApplicationUser> spec, IConfigurationProvider mapperConfig);

        /// <summary>
        /// Counts the number of users that match a given specification asynchronously.
        /// </summary>
        /// <param name="spec">The specification to filter users.</param>
        /// <returns>The count of users that match the specification.</returns>
        Task<int> CountAsync(ISpecification<ApplicationUser> spec);

        /// <summary>
        /// Inserts a new user with the specified password asynchronously.
        /// </summary>
        /// <param name="user">The user to insert.</param>
        /// <param name="password">The password for the new user.</param>
        Task InsertUserAsync(ApplicationUser user, string password);

        /// <summary>
        /// Assigns a role to a user asynchronously.
        /// </summary>
        /// <param name="user">The user to assign the role to.</param>
        /// <param name="role">The role to assign.</param>
        Task AssignRoleAsync(ApplicationUser user, string role);

        /// <summary>
        /// Updates an existing user's information asynchronously.
        /// </summary>
        /// <param name="user">The user with updated information.</param>
        Task UpdateAsync(ApplicationUser user);

        /// <summary>
        /// Retrieves a user by their email address asynchronously.
        /// </summary>
        /// <param name="email">The email address to search for.</param>
        /// <returns>The user with the specified email, or null if not found.</returns>
        Task<ApplicationUser?> GetUserByEmailAsync(string email, bool asNoTracking = true);

        /// <summary>
        /// Gets the role of a specified user asynchronously.
        /// </summary>
        /// <param name="user">The user whose role to retrieve.</param>
        /// <returns>The role of the user.</returns>
        Task<string> GetUserRoleAsync(ApplicationUser user);

        /// <summary>
        /// Retrieves a user by their ID asynchronously, with optional navigation property includes.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <param name="includes">A list of navigation property names to include, or null to disable includes.</param>
        /// <param name="isTracking">Whether to use tracking or not.</param>
        /// <returns>The user with the specified ID, or null if not found.</returns>
        Task<ApplicationUser?> GetByIdAsync(Guid userId, List<string>? includes = null, bool isTracking = false);

        /// <summary>
        /// Generates an email confirmation token for a user asynchronously.
        /// </summary>
        /// <param name="user">The user to generate the token for.</param>
        /// <returns>The generated token.</returns>
        Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);

        Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token);

        Task<List<string>> GetUserRolesAsync(ApplicationUser user);

        Task<bool> IsInRoleAsync(ApplicationUser user, string role);

        Task<bool> UpdateLoginInfoAsync(ApplicationUser user, string? email, string? phoneNumber);

        Task<bool> UpdatePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);

        #endregion Core methods

        Task UpdateUserPresence(Guid userId, bool isOnline);
    }
}