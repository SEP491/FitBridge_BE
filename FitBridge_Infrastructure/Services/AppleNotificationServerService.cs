using System;
using System.Text;
using System.Text.Json;
using FitBridge_Application.Dtos.Payments.ApplePaymentDto;
using FitBridge_Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace FitBridge_Infrastructure.Services;

public class AppleNotificationServerService(ILogger<AppleNotificationServerService> _logger, ITransactionService _transactionService) : IAppleNotificationServerService
{
    public async Task<bool> HandleAppleWebhookAsync(string webhookData)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            _logger.LogInformation("Received apple webhook data: " + webhookData);
            var decodedPayload = JsonSerializer.Deserialize<AppStoreServerNotificationV2>(webhookData, options);
            if (decodedPayload == null)
            {
                _logger.LogWarning("Invalid webhook payload received");
                return false;
            }
            _logger.LogInformation("Decoded payload: " + JsonSerializer.Serialize(decodedPayload, options));
            var signedPayload = decodedPayload.SignedPayload;

            var decodedSignedPayload = DecodeWithoutVerifyAsn(signedPayload);
            _logger.LogInformation("Decoded signed payload: " + JsonSerializer.Serialize(decodedSignedPayload, options));
            var jwsHeader = decodedSignedPayload.header;
            var asnDecodedPayload = decodedSignedPayload.body;

            var decodedSignTransactionInfo = DecodeWithoutVerifySignTransaction(asnDecodedPayload.Data.SignedTransactionInfo);
            if (decodedSignTransactionInfo == null)
            {
                _logger.LogWarning("Invalid signed transaction info received");
                return false;
            }
            _logger.LogInformation("Decoded signed transaction info: " + JsonSerializer.Serialize(decodedSignTransactionInfo, options));
            var decodedSignRenewalInfo = new JwsRenewalInfoDecoded();
            if (asnDecodedPayload.Data.SignedRenewalInfo != null)
            {
                decodedSignRenewalInfo = DecodeWithoutVerifySignRenewal(asnDecodedPayload.Data.SignedRenewalInfo);
                _logger.LogInformation("Decoded signed renewal info: " + JsonSerializer.Serialize(decodedSignRenewalInfo, options));
            }
            if (asnDecodedPayload.NotificationType == "SUBSCRIBED")
            {
                await _transactionService.PurchaseAppleSubscriptionPlans(asnDecodedPayload, decodedSignTransactionInfo);
            }
            var transactionId = decodedSignTransactionInfo.TransactionId;
            var originalTransactionId = decodedSignTransactionInfo.OriginalTransactionId;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling Apple webhook");
            return false;
        }
    }

    static byte[] Base64UrlDecode(string s)
    {
        s = s.Replace('-', '+').Replace('_', '/');
        switch (s.Length % 4) { case 2: s += "=="; break; case 3: s += "="; break; }
        return Convert.FromBase64String(s);
    }

    public static (JwsHeader header, AsnDecodedPayload body) DecodeWithoutVerifyAsn(string signedPayload)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var parts = signedPayload.Split('.');
        var headerJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[0]));
        var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));

        var header = JsonSerializer.Deserialize<JwsHeader>(headerJson, options)!;
        var body = JsonSerializer.Deserialize<AsnDecodedPayload>(payloadJson, options)!;
        return (header, body);
    }

    public static JwsTransactionDecoded DecodeWithoutVerifySignTransaction(string signedPayload)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var parts = signedPayload.Split('.');
        var headerJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[0]));
        var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));

        var header = JsonSerializer.Deserialize<JwsHeader>(headerJson, options)!;
        var body = JsonSerializer.Deserialize<JwsTransactionDecoded>(payloadJson, options)!;
        return body;
    }

    public static JwsRenewalInfoDecoded DecodeWithoutVerifySignRenewal(string signedPayload)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var parts = signedPayload.Split('.');
        var headerJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[0]));
        var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));

        var header = JsonSerializer.Deserialize<JwsHeader>(headerJson, options)!;
        var body = JsonSerializer.Deserialize<JwsRenewalInfoDecoded>(payloadJson, options)!;
        return body;
    }
}
