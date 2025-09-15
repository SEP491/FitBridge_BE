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
            return await userManager.Users.OfType<ApplicationUser>().ToListAsync();
        }

        public async Task<IList<ApplicationUser>> GetUsersByRoleAsync(string role)
        {
            return await userManager.GetUsersInRoleAsync(role);
        }

        public async Task<IReadOnlyList<ApplicationUser>> GetAllUsersWithSpecAsync(ISpecification<ApplicationUser> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<ApplicationUser?> GetUserWithSpecAsync(ISpecification<ApplicationUser> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<TDto?> GetUserWithSpecProjectedAsync<TDto>(ISpecification<ApplicationUser> spec, IConfigurationProvider mapperConfig)
        {
            return await ApplySpecification(spec).ProjectTo<TDto>(mapperConfig).FirstOrDefaultAsync();
        }

        public async Task<List<TDto>> GetAllUserWithSpecProjectedAsync<TDto>(ISpecification<ApplicationUser> spec, IConfigurationProvider mapperConfig)
        {
            return await ApplySpecification(spec).ProjectTo<TDto>(mapperConfig).ToListAsync();
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

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        public async Task<string> GetUserRoleAsync(ApplicationUser user)
        {
            return (await userManager.GetRolesAsync(user)).FirstOrDefault() ?? string.Empty;
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
            var existingUserByPhoneNumber = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == user.PhoneNumber);
            if (existingUserByPhoneNumber != null)
            {
                throw new DuplicateUserException($"A user with the phone number {user.PhoneNumber} already exists.");
            }

            var existingUserByEmail = await userManager.FindByEmailAsync(user.Email!);
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
    }
}