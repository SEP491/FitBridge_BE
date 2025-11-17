using System.Text.Json;
using FitBridge_Application.Dtos.Shippings;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FitBridge_Application.Features.Orders.ProcessAhamoveWebhook;

public class ProcessAhamoveWebhookCommandHandler : IRequestHandler<ProcessAhamoveWebhookCommand, bool>
{
    private readonly IAhamoveService _ahamoveService;
    private readonly ILogger<ProcessAhamoveWebhookCommandHandler> _logger;

    public ProcessAhamoveWebhookCommandHandler(
        IAhamoveService ahamoveService,
        ILogger<ProcessAhamoveWebhookCommandHandler> logger)
    {
        _ahamoveService = ahamoveService;
        _logger = logger;
    }

    public async Task<bool> Handle(ProcessAhamoveWebhookCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"Received webhook payload: {request.WebhookPayload}");

            // Then, deserialize the inner data string to get the actual Ahamove webhook data
            var webhookData = JsonSerializer.Deserialize<AhamoveWebhookDto>(request.WebhookPayload, new JsonSerializerOptions
            {
                // PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            });

            if (webhookData == null)
            {
                throw new BusinessException("Invalid webhook payload: could not deserialize inner data");
            }

            // Process the webhook data
            await _ahamoveService.ProcessWebhookAsync(webhookData);

            _logger.LogInformation($"Successfully processed webhook for Ahamove Order ID: {webhookData.Id}");
            return true;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, $"Failed to parse webhook payload: {request.WebhookPayload}");
            throw new BusinessException($"Failed to parse webhook payload: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Ahamove webhook");
            throw;
        }
    }
}

