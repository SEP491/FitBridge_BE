using System.Linq.Expressions;

namespace FitBridge_Application.Interfaces.Specifications
{
    public interface ISpecification<T> where T : class
    {
        Expression<Func<T, bool>>? Criteria { get; } // WHERE

        Expression<Func<T, object>>? OrderBy { get; }

        Expression<Func<T, object>>? OrderByDescending { get; }

        Expression<Func<T, object>>? GroupBy { get; }

        Expression<Func<IGrouping<object, T>, bool>>? Having { get; }

        Expression<Func<IGrouping<object, T>, T>>? Select { get; } // SELECT

        List<Expression<Func<T, object>>> Includes { get; }// LEFT JOIN

        List<string> IncludeStrings { get; } // Orders.Include("OrderDetails.Person")

        public int? Top { get; } // Get top N records

        public int Skip { get; } // Skip N records

        public int Take { get; } // Take N records

        public bool IsPagingEnabled { get; } // Enable paging

        public bool IsDistinct { get; } // Enable distinct records
    }
}