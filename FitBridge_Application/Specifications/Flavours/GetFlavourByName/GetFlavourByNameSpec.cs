using FitBridge_Domain.Entities.Ecommerce;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Flavours.GetFlavourByName
{
    public class GetFlavourByNameSpec : BaseSpecification<Flavour>
    {
        public GetFlavourByNameSpec(string name) : base(x =>
            x.IsEnabled
            && x.Name.ToLower().Equals(name.ToLower()))
        {
        }
    }
}