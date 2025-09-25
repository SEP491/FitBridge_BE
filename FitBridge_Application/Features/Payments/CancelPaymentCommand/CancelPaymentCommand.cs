using System;
using MediatR;

namespace FitBridge_Application.Features.Payments.CancelPaymentCommand;

public class CancelPaymentCommand : IRequest<bool>
{
    public long OrderCode { get; set; }
}
