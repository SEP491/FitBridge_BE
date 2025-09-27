using System;
using MediatR;
using FitBridge_Application.Dtos.Identities;
using FitBridge_Domain.Entities.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Specifications.Accounts;

namespace FitBridge_Application.Features.Identities.Login;

public class LoginUserCommandHandler(
        ILogger<LoginUserCommandHandler> logger,
        SignInManager<ApplicationUser> signInManager,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IApplicationUserService applicationUserService,
        IUserTokenService userTokenService
    ) : IRequestHandler<LoginUserCommand, LoginResponseDTO>
{
    public async Task<LoginResponseDTO> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await GetAndValidateUser(request.Identifier);
            ValidatePassword(user, request.Password);
            await SignInUser(user, request.Password);
            var userRoles = await GetUserRole(user);
            var tokens = await GenerateTokens(user, userRoles);

            logger.LogInformation("Login successful for user {UserId} with roles {Roles}", user.Id, userRoles);

            return new LoginResponseDTO
            {
                AccessToken = tokens.accessToken,
                IdToken = tokens.idToken,
                RefreshToken = tokens.refreshToken,
                Id = user.Id,
                FullName = user.FullName,
                Phone = user.PhoneNumber,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl
            };
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves and validates a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user to find.</param>
    /// <returns>The found ApplicationUser if exists.</returns>
    /// <exception cref="BadHttpRequestException">Thrown when user is not found.</exception>
    private async Task<ApplicationUser> GetAndValidateUser(string identifier)
    {
        ApplicationUser? user = await applicationUserService.GetUserWithSpecAsync(new GetAccountByIdentifierSpecification(identifier));
        if (user is null)
        {
            logger.LogWarning("User not found with identifier: {Identifier}", identifier);
            throw new Exception($"User with identifier: {identifier} was not found");
        }

        if (!user.EmailConfirmed)
        {
            throw new Exception("User email is not verified");
        }

        logger.LogDebug("User found: {UserId}", user.Id);
        return user;
    }

    /// <summary>
    /// Validates the provided password against the user's stored password hash.
    /// </summary>
    /// <param name="user">The user whose password needs to be validated.</param>
    /// <param name="password">The password to validate.</param>
    /// <exception cref="BadHttpRequestException">Thrown when password validation fails.</exception>
    private void ValidatePassword(ApplicationUser user, string password)
    {
        logger.LogDebug("Validating password for user: {UserId}", user.Id);

        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            logger.LogWarning("Password validation failed for user: {UserId}", user.Id);
            throw new Exception("Incorrect password");
        }

        logger.LogDebug("Password validation successful for user: {UserId}", user.Id);
    }

    /// <summary>
    /// Attempts to sign in the user using the provided credentials.
    /// </summary>
    /// <param name="user">The user attempting to sign in.</param>
    /// <param name="password">The password for authentication.</param>
    /// <returns>A task representing the asynchronous sign-in operation.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when sign-in fails.</exception>
    private async Task SignInUser(ApplicationUser user, string password)
    {
        logger.LogDebug("Attempting to sign in user: {UserId}", user.Id);

        //TODO: maybe add 'remember me' option
        var result = await signInManager.PasswordSignInAsync(
            user,
            password,
            isPersistent: false,
            lockoutOnFailure: false);

        if (!result.Succeeded) //result may not succeed due to invalid 2FA code, not just incorrect password.
        {
            logger.LogWarning("Sign in failed for user: {UserId}. Result: {@SignInResult}", user.Id, result);
            throw new UnauthorizedAccessException("Your authentication attempt failed, please try again with valid credentials");
        }

        logger.LogDebug("Sign in successful for user: {UserId}", user.Id);
    }

    /// <summary>
    /// Retrieves the first role assigned to the user.
    /// </summary>
    /// <param name="user">The user whose role needs to be retrieved.</param>
    /// <returns>The user's first assigned role.</returns>
    /// <remarks>Assumes the user has at least one role assigned.</remarks>
    private async Task<List<string>> GetUserRole(ApplicationUser user)
    {
        logger.LogDebug("Retrieving roles for user: {UserId}", user.Id);

        var userRoles = await applicationUserService.GetUserRolesAsync(user);

        logger.LogDebug("Retrieved roles {Roles} for user: {UserId}", userRoles, user.Id);
        return userRoles;
    }

    /// <summary>
    /// Generates access, ID, and refresh tokens for the authenticated user.
    /// </summary>
    /// <param name="user">The user to generate tokens for.</param>
    /// <param name="userRole">The user's role to include in the access token.</param>
    /// <returns>A tuple containing the access token, ID token, and refresh token.</returns>
    /// <remarks>Updates the user's refresh token in the database.</remarks>
    private async Task<(string accessToken, string idToken, string refreshToken)> GenerateTokens(ApplicationUser user, List<string> userRoles)
    {
        logger.LogDebug("Generating tokens for user: {UserId} with roles: {Roles}", user.Id, userRoles);

        string accessToken = userTokenService.CreateAccessToken(user, userRoles);
        string idToken = userTokenService.CreateIdToken(user, userRoles);
        string refreshToken = userTokenService.CreateRefreshToken(user);

        user.RefreshToken = refreshToken;

        logger.LogDebug("Tokens generated successfully for user: {UserId}", user.Id);
        return (accessToken, idToken, refreshToken);
    }
}