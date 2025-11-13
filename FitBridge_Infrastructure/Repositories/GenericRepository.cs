using AutoMapper;
using AutoMapper.QueryableExtensions;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Specifications;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities;
using FitBridge_Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitBridge_Infrastructure.Repositories
{
    internal class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        // ===========================
        // === Fields & Props
        // ===========================

        private readonly FitBridgeDbContext _dbContext;

        // ===========================
        // === Constructors
        // ===========================

        public GenericRepository(FitBridgeDbContext context)
        {
            _dbContext = context;
        }

        // ===========================
        // === INSERT, UPDATE, DELETE
        // ===========================

        public T Insert(T entity)
        {
            var addedEntity = _dbContext.Set<T>().Add(entity).Entity;
            return addedEntity;
        }

        public void InsertRange(List<T> entities)
        {
            _dbContext.Set<T>().AddRange(entities);
        }

        public T? Update(T entityToUpdate)
        {
            _dbContext.Entry(entityToUpdate).State = EntityState.Modified;
            return entityToUpdate;
        }

        public T? Delete(T entityToDelete)
        {
            var deletedEntity = _dbContext.Set<T>().Remove(entityToDelete).Entity;
            return deletedEntity;
        }

        public T? Delete(object id)
        {
            var entityToDelete = _dbContext.Set<T>().Find(id);
            return entityToDelete == null ? null : Delete(entityToDelete);
        }

        public T? SoftDelete(T entityToDelete)
        {
            _dbContext.Entry(entityToDelete).State = EntityState.Modified;
            entityToDelete.IsEnabled = false;
            return entityToDelete;
        }

        public T? SoftDelete(object id)
        {
            var entityToDelete = _dbContext.Set<T>().Find(id);
            return entityToDelete == null ? null : SoftDelete(entityToDelete);
        }

        // ========================================
        // === GET queries with Specification
        // === Using with Include and .ThenInclude
        // ========================================

        public async Task<IReadOnlyList<T>> GetAllWithSpecificationAsync(ISpecification<T> specification, bool asNoTracking = true)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsQueryable();
            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }
            return await SpecificationQueryBuilder<T>.BuildQuery(query, specification).ToListAsync();
        }

        public async Task<bool> AnyAsync(ISpecification<T> specification)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsQueryable().AsNoTracking();
            return await SpecificationQueryBuilder<T>.BuildQuery(query, specification).AnyAsync();
        }

        public async Task<T?> GetBySpecificationAsync(ISpecification<T> specification, bool asNoTracking = true)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsQueryable();
            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }
            return await SpecificationQueryBuilder<T>.BuildQuery(query, specification).FirstOrDefaultAsync();
        }

        public async Task<int> CountAsync(ISpecification<T> specification)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsQueryable();
            return await SpecificationQueryBuilder<T>.BuildCountQuery(query, specification).CountAsync();
        }

        // ===========================================
        // === GET queries Projection with AutoMapper
        // === Using with Profile and Dto
        // ===========================================
        public async Task<TDto?> GetBySpecificationProjectedAsync<TDto>(ISpecification<T> specification, IConfigurationProvider mapperConfig)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsQueryable();
            return await SpecificationQueryBuilder<T>.BuildQuery(query, specification)
                .ProjectTo<TDto>(mapperConfig)
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<TDto>> GetAllWithSpecificationProjectedAsync<TDto>(ISpecification<T> specification, IConfigurationProvider mapperConfig)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsQueryable();
            return await SpecificationQueryBuilder<T>.BuildQuery(query, specification)
                .ProjectTo<TDto>(mapperConfig)
                .ToListAsync();
        }

        // ===========================================
        // === GET queries with GROUP BY
        // ===========================================

        public async Task<IReadOnlyList<T>> GetAllGroupByAsync(ISpecification<T> specification)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsQueryable();
            return await SpecificationQueryBuilder<T>.BuildGroupByQuery(query, specification).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id, bool asNoTracking = true, List<string>? includes = null)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include.ToString()));
            }
            return await query.FirstOrDefaultAsync(e => e.Id == id && e.IsEnabled == true);
        }

        public async Task<TDto?> GetByIdProjectedAsync<TDto>(Guid id, IConfigurationProvider mapperConfig)
        {
            return await _dbContext.Set<T>()
                .Where(e => e.Id == id && e.IsEnabled == true)
                .ProjectTo<TDto>(mapperConfig)
                .FirstOrDefaultAsync();
        }
    }
}