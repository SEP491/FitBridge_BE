using System;
using FitBridge_Application.Dtos.Payments;
using MediatR;

namespace FitBridge_Application.Features.Payments.RePaidOrder;

public class RePaidOrderCommand : IRequest<string>  
{
    public Guid OrderId { get; set; }
}
