using AutoMapper;
using FitBridge_Application.Interfaces.Specifications;
using FitBridge_Domain.Entities;

namespace FitBridge_Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {

        // GET
        Task<T?> GetByIdAsync(Guid id, bool asNoTracking = true);
        Task<IReadOnlyList<T>> GetAllWithSpecificationAsync(ISpecification<T> specification, bool asNoTracking = true);

        Task<T?> GetBySpecificationAsync(ISpecification<T> specification, bool asNoTracking = true);

        Task<int> CountAsync(ISpecification<T> specification);

        // GET support Projection with Automapper for better performance
        Task<TDto?> GetBySpecificationProjectedAsync<TDto>(ISpecification<T> specification, IConfigurationProvider mapperConfig);
        Task<TDto?> GetByIdProjectedAsync<TDto>(Guid id, IConfigurationProvider mapperConfig);
        Task<IReadOnlyList<TDto>> GetAllWithSpecificationProjectedAsync<TDto>(ISpecification<T> specification, IConfigurationProvider mapperConfig);

        // INSERT, DELETE, UPDATE
        public T Insert(T entity);

        public T? Update(T entityToUpdate);

        public T? Delete(T entityToDelete);

        public T? Delete(object id);

        public T? SoftDelete(T entityToDelete);

        public T? SoftDelete(object id);
    }
}