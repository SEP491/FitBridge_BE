using System;
using FitBridge_Application.Specifications.Transactions;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Specifications.GymCoursePts.GetGymCoursePtById;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Services;

public class TransactionsService(IUnitOfWork _unitOfWork) : ITransactionService
{
    public async Task<bool> ExtendCourse(long orderCode)
    {
        var transactionToExtend = await _unitOfWork.Repository<FitBridge_Domain.Entities.Orders.Transaction>().GetBySpecificationAsync(new GetTransactionByOrderCodeWithIncludeSpec(orderCode), false);
        if (transactionToExtend == null)
        {
            throw new NotFoundException("Transaction not found with order code " + orderCode);
        }
        var orderItemToExtend = transactionToExtend.Order.OrderItems.First();
        var customerPurchasedToExtend = transactionToExtend.Order.CustomerPurchasedToExtend;
        orderItemToExtend.CustomerPurchasedId = customerPurchasedToExtend.Id;
        var numOfSession = 0;
        if (orderItemToExtend.FreelancePTPackageId != null)
        {
            numOfSession = orderItemToExtend.FreelancePTPackage.NumOfSessions;
        }
        if (orderItemToExtend.GymCourseId != null && orderItemToExtend.GymPtId != null)
        {
            var gymCoursePT = await _unitOfWork.Repository<GymCoursePT>().GetBySpecificationAsync(new GetGymCoursePtByGymCourseIdAndPtIdSpec(orderItemToExtend.GymCourseId.Value, orderItemToExtend.GymPtId.Value));
            if (gymCoursePT == null)
            {
                throw new NotFoundException("Gym course PT with gym course id and pt id not found");
            }
            numOfSession = gymCoursePT.Session.Value;
        }
        customerPurchasedToExtend.AvailableSessions += orderItemToExtend.Quantity * numOfSession;
        customerPurchasedToExtend.ExpirationDate = customerPurchasedToExtend.ExpirationDate.AddDays(30 * orderItemToExtend.Quantity);
        transactionToExtend.Order.Status = OrderStatus.Arrived;
        
        await _unitOfWork.CommitAsync();
        return true;
    }

}
