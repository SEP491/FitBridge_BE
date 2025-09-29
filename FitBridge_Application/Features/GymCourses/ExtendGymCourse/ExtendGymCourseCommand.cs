using System;
using FitBridge_Application.Dtos.Payments;
using MediatR;

namespace FitBridge_Application.Features.GymCourses.ExtendGymCourse;

public class ExtendGymCourseCommand : IRequest<PaymentResponseDto>
{
    public Guid CustomerPurchasedIdToExtend { get; set; }
    public Guid PaymentMethodId { get; set; }
    public int Quantity { get; set; }
}
