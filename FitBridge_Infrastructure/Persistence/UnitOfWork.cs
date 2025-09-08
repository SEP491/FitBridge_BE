using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities;
using FitBridge_Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace FitBridge_Infrastructure.Persistence
{
    internal class UnitOfWork(FitBridgeDbContext dbContext) : IUnitOfWork
    {
        // ===================================
        // === Fields & Prop
        // ===================================

        private readonly ConcurrentDictionary<string, object> _repositoryDictionary = new();

        private bool _disposed = false;

        // ===================================
        // === Methods
        // ===================================

        /// <summary>
        ///     Dispose object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    dbContext.Dispose();
                }
                _disposed = true;
            }
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }

        public IExecutionStrategy CreateExecutionStrategy() => dbContext.Database.CreateExecutionStrategy();

        public async Task<int> CommitAsync() => await dbContext.SaveChangesAsync();

        public IGenericRepository<T> Repository<T>() where T : BaseEntity
        {
            var typeEntityName = typeof(T).Name;

            var repoInstanceTypeT = _repositoryDictionary.GetOrAdd(typeEntityName,
                valueFactory: _ =>
                {
                    var repoType = typeof(GenericRepository<T>);

                    var repoInstance = Activator.CreateInstance(repoType, dbContext);

                    return repoInstance!;
                });

            return (IGenericRepository<T>)repoInstanceTypeT;
        }
    }
}