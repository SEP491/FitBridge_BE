using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Specifications.PaymentMethods;

public class GetPaymentMethodByNameSpec : BaseSpecification<PaymentMethod>
{
    public GetPaymentMethodByNameSpec(MethodType methodType) : base(x => x.MethodType == methodType)
    {
    }
}
