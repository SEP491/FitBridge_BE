using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace FitBridge_Application.Features.Sample
{
    internal class SampleCommand2Handler(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork) : IRequestHandler<SampleCommand2>
    {
        public async Task Handle(SampleCommand2 request, CancellationToken cancellationToken)
        {
            var strategy = unitOfWork.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
                try
                {
                    var user = new IdentityUser
                    {
                        UserName = request.UserName,
                        Email = request.Email
                    };

                    var result = await userManager.CreateAsync(user, request.Password);

                    if (!result.Succeeded)
                    {
                        // Handle creation failure
                        throw new InvalidOperationException($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                    // uow.CommitAsync()
                    var user2 = new IdentityUser
                    {
                        UserName = $"{request.UserName}_2",
                        Email = $"2_{request.Email}"
                    };

                    var result2 = await userManager.CreateAsync(user2, request.Password);

                    if (!result2.Succeeded)
                    {
                        // Handle creation failure for second user
                        throw new InvalidOperationException($"Second user creation failed: {string.Join(", ", result2.Errors.Select(e => e.Description))}");
                    }
                    // uow.CommitAsync()

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    // Transaction will automatically rollback when scope is disposed without Complete()
                    throw new InvalidOperationException($"Transaction failed: {ex.Message}", ex);
                }
            });
        }
    }
}