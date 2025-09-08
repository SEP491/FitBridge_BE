using FitBridge_Application.Interfaces.Specifications;
using Microsoft.EntityFrameworkCore;

namespace FitBridge_Application.Specifications
{
    public static class SpecificationQueryBuilder<T> where T : class
    {
        public static IQueryable<T> BuildQuery(IQueryable<T> query, ISpecification<T> specification)
        {
            // Where
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            // Order
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }

            if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            if (specification.Includes.Count > 0)
            {
                query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));
            }

            if (specification.IncludeStrings.Count > 0)
            {
                query = specification.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));
            }

            if (specification.Top != null)
            {
                query = query.Take(specification.Top.Value);
            }

            if (specification.IsPagingEnabled)
            {
                query = query.Skip(specification.Skip).Take(specification.Take);
            }

            return query;
        }

        /// <summary>
        /// Get Count Entities Query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        public static IQueryable<T> BuildCountQuery(IQueryable<T> query, ISpecification<T> specification)
        {
            // WHERE x.Brand = "Hitachi"
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            // DISTINCT
            // - Distinct can affect performance, recommend not using
            if (specification.IsDistinct)
            {
                query = query.Distinct();
            }

            return query;
        }

        /// <summary>
        /// Build the specific group by query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        public static IQueryable<T> BuildGroupByQuery(IQueryable<T> query, ISpecification<T> specification)
        {
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            // Group By, Having, Select
            if (specification.GroupBy != null)
            {
                var groupedByQuery = query.GroupBy(specification.GroupBy);

                // Having
                if (specification.Having != null)
                {
                    groupedByQuery = groupedByQuery.Where(specification.Having);
                }

                var resultQuery = groupedByQuery.Select(specification.Select!);

                if (specification.OrderBy != null)
                {
                    resultQuery = resultQuery.OrderBy(specification.OrderBy!);
                }

                if (specification.OrderByDescending != null)
                {
                    resultQuery = resultQuery.OrderByDescending(specification.OrderByDescending);
                }

                if (specification.IsPagingEnabled)
                {
                    resultQuery = resultQuery.Skip(specification.Skip).Take(specification.Take);
                }

                return resultQuery;
            }

            return query;
        }
    }
}