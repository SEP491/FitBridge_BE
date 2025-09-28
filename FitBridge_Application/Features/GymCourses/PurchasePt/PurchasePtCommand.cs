using System;
using FitBridge_Application.Dtos.Payments;
using MediatR;
 
namespace FitBridge_Application.Features.GymCourses.PurchasePt;

public class PurchasePtCommand : IRequest<PaymentResponseDto>
{
    public Guid GymCoursePTId { get; set; }
    public Guid CustomerPurchasedId { get; set; }
    public Guid PaymentMethodId { get; set; }
}
