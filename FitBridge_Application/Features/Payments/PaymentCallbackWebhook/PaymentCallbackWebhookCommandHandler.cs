using System;
using MediatR;
using FitBridge_Application.Interfaces.Services;

namespace FitBridge_Application.Features.Payments.PaymentCallbackWebhook;

// public class PaymentCallbackWebhookCommandHandler(IPayOSService _payOSService) : IRequestHandler<PaymentCallbackWebhookCommand, bool>
// {
//     public async Task<bool> Handle(PaymentCallbackWebhookCommand request, CancellationToken cancellationToken)
//     {
//         var result = await _payOSService.HandlePaymentWebhookAsync(request.WebhookData);
//         return result;
//     }
// }
