using System;
using FitBridge_Application.Dtos.Payments;
using MediatR;

namespace FitBridge_Application.Features.GymCourses.ExtendGymCourse;

public class ExtendGymCourseCommand : IRequest<PaymentResponseDto>
{
    public CreatePaymentRequestDto Request { get; set; }
}
