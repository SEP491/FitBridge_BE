using System;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Application.Specifications.PaymentMethods;
using FitBridge_Application.Interfaces.Repositories;
using System.Threading.Tasks;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Commons.Utils;

public static class GetSystemPaymentMethodId
{
    public static async Task<Guid> GetPaymentMethodId(MethodType methodType, IUnitOfWork _unitOfWork)
    {
        var paymentMethod = await _unitOfWork.Repository<PaymentMethod>().GetBySpecificationAsync(new GetPaymentMethodByNameSpec(methodType));
        if(paymentMethod == null)
        {
            throw new NotFoundException("Payment method not found");
        }
        return paymentMethod.Id;
    }
}
