using System;
using FitBridge_Domain.Enums.Reviews;

namespace FitBridge_Application.Specifications.Reviews.GetAllReviewForCustomer;

public class GetCustomerReviewParams : BaseParams
{
    public Guid? CustomerId { get; set; }
    public ReviewType? ReviewType { get; set; }
}
