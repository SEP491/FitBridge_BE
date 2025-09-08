using FitBridge_Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace FitBridge_Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        public IGenericRepository<T> Repository<T>() where T : BaseEntity;

        IExecutionStrategy CreateExecutionStrategy();

        Task<int> CommitAsync();
    }
}