using System;

namespace FitBridge_Application.Features.Payments.AppleWebhook;

public class AppleWebhookCommandHandler(IPayOSService _payOSService) : IRequestHandler<AppleWebhookCommand, bool>
{

}
