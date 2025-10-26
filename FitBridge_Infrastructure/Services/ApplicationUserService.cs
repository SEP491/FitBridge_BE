using AutoMapper;
using AutoMapper.QueryableExtensions;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Specifications;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FitBridge_Infrastructure.Services
{
    internal class ApplicationUserService(
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork) : IApplicationUserService
    {
        public async Task<IReadOnlyList<ApplicationUser>> GetAllUsersAsync()
        {
            return await userManager.Users.OfType<ApplicationUser>().AsNoTracking().ToListAsync();
        }

        public async Task<IList<ApplicationUser>> GetUsersByRoleAsync(string role)
        {
            return await userManager.GetUsersInRoleAsync(role);
        }

        public async Task<IReadOnlyList<ApplicationUser>> GetAllUsersWithSpecAsync(ISpecification<ApplicationUser> spec)
        {
            return await ApplySpecification(spec).AsNoTracking().ToListAsync();
        }

        public async Task<ApplicationUser?> GetUserWithSpecAsync(ISpecification<ApplicationUser> spec, bool asNoTracking = true)
        {
            var query = ApplySpecification(spec);
            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<TDto?> GetUserWithSpecProjectedAsync<TDto>(ISpecification<ApplicationUser> spec, IConfigurationProvider mapperConfig, bool asNoTracking = true)
        {
            var query = ApplySpecification(spec);
            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.ProjectTo<TDto>(mapperConfig).FirstOrDefaultAsync();
        }

        public async Task<List<TDto>> GetAllUserWithSpecProjectedAsync<TDto>(ISpecification<ApplicationUser> spec, IConfigurationProvider mapperConfig)
        {
            return await ApplySpecification(spec).AsNoTracking().ProjectTo<TDto>(mapperConfig).ToListAsync();
        }

        public async Task<int> CountAsync(ISpecification<ApplicationUser> spec)
        {
            var query = userManager.Users.OfType<ApplicationUser>().AsQueryable();
            return await SpecificationQueryBuilder<ApplicationUser>.BuildCountQuery(query, spec).CountAsync();
        }

        private IQueryable<ApplicationUser> ApplySpecification(ISpecification<ApplicationUser> spec)
        {
            var query = userManager.Users.OfType<ApplicationUser>().AsQueryable();
            return SpecificationQueryBuilder<ApplicationUser>.BuildQuery(query, spec);
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            await userManager.UpdateAsync(user);
        }

        public async Task AssignRoleAsync(ApplicationUser user, string role)
        {
            await userManager.AddToRoleAsync(user, role);
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email, bool asNoTracking = true)
        {
            if (!asNoTracking)
            {
                return await userManager.FindByEmailAsync(email);
            }

            var normalizedEmail = email.ToUpperInvariant();
            return await userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
        }

        public async Task<string> GetUserRoleAsync(ApplicationUser user)
        {
            return (await userManager.GetRolesAsync(user)).FirstOrDefault() ?? string.Empty;
        }

        public async Task<List<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return (await userManager.GetRolesAsync(user)).ToList();
        }

        public async Task<ApplicationUser?> GetByIdAsync(Guid userId, List<string>? includes = null, bool isTracking = false)
        {
            var query = userManager.Users.AsQueryable();
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include.ToString()));
            }

            return isTracking ? await query.FirstOrDefaultAsync(u => u.Id == userId) : await query.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task InsertUserAsync(ApplicationUser user, string password)
        {
            var existingUserByPhoneNumber = await userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.PhoneNumber == user.PhoneNumber);
            if (existingUserByPhoneNumber != null)
            {
                throw new DuplicateUserException($"A user with the phone number {user.PhoneNumber} already exists.");
            }

            var existingUserByEmail = await userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUserByEmail != null)
            {
                throw new DuplicateUserException($"A user with the email {user.Email} already exists.");
            }

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new CreateFailedException($"User creation failed: {errors}");
            }
        }

        public async Task<ApplicationUser?> GetUserByPhoneNumberAsync(string phoneNumber, bool asNoTracking = true)
        {
            var query = userManager.Users.AsQueryable();
            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            return await userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token)
        {
            return await userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string role)
        {
            return await userManager.IsInRoleAsync(user, role);
        }

        public async Task<bool> UpdateLoginInfoAsync(ApplicationUser user, string? email, string? phoneNumber)
        {

            if (email != null)
            {
                var existingUserByEmail = await userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email && u.Id != user.Id);
                if (existingUserByEmail != null)
                {
                    throw new DuplicateUserException($"A user with the email {email} already exists.");
                }
                var updateEmailResult = await userManager.SetEmailAsync(user, email);
                if (!updateEmailResult.Succeeded)
                {
                    throw new UpdateFailedException($"Failed to update email: {string.Join(", ", updateEmailResult.Errors.Select(e => e.Description))}");
                }
                var updateUserNameResult = await userManager.SetUserNameAsync(user, email);
                if (!updateUserNameResult.Succeeded)
                {
                    throw new UpdateFailedException($"Failed to update user name: {string.Join(", ", updateUserNameResult.Errors.Select(e => e.Description))}");
                }
                var confirmEmailResult = await userManager.ConfirmEmailAsync(user, await userManager.GenerateEmailConfirmationTokenAsync(user));
                if (!confirmEmailResult.Succeeded)
                {
                    throw new UpdateFailedException($"Failed to confirm email: {string.Join(", ", confirmEmailResult.Errors.Select(e => e.Description))}");
                }
            }
            if (phoneNumber != null)
            {
                var existingUserByPhoneNumber = await userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber && u.Id != user.Id);
                if (existingUserByPhoneNumber != null)
                {
                    throw new DuplicateUserException($"A user with the phone number {phoneNumber} already exists.");
                }
                var updatePhoneNumberResult = await userManager.SetPhoneNumberAsync(user, phoneNumber);
                if(!updatePhoneNumberResult.Succeeded)
                {
                    throw new UpdateFailedException($"Failed to update phone number: {string.Join(", ", updatePhoneNumberResult.Errors.Select(e => e.Description))}");
                }
            }
            return true;
        }

        public async Task<bool> UpdatePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
            var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                throw new UpdateFailedException($"Failed to update password: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            return true;
        }
    }
}