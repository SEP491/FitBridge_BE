using FitBridge_Domain.Entities.Ecommerce;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Weights.GetWeightByValueAndUnit
{
    public class GetWeightByValueAndUnitSpec : BaseSpecification<Weight>
    {
        public GetWeightByValueAndUnitSpec(double value, string unit) : base(x =>
            x.Value == value && x.Unit == unit)
        {
        }
    }
}
