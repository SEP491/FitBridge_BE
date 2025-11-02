using System;
using System.Text;
using System.Text.Json;
using FitBridge_Application.Dtos.Payments.ApplePaymentDto;
using FitBridge_Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace FitBridge_Infrastructure.Services;

public class AppleNotificationServerService(ILogger<AppleNotificationServerService> _logger) : IAppleNotificationServerService
{
    public async Task<bool> HandleAppleWebhookAsync(string webhookData)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var decodedPayload = JsonSerializer.Deserialize<AppStoreServerNotificationV2>(webhookData, options);
            if (decodedPayload == null)
            {
                _logger.LogWarning("Invalid webhook payload received");
                return false;
            }

            var signedPayload = decodedPayload.SignedPayload;

            var decodedSignedPayload = DecodeWithoutVerify(signedPayload);
            var jwsHeader = decodedSignedPayload.header;
            var asnDecodedPayload = decodedSignedPayload.body;

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
    public static (JwsHeader header, AsnDecodedPayload body) DecodeWithoutVerify(string signedPayload)
    {
        var parts = signedPayload.Split('.');
        var headerJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[0]));
        var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));

        var header = JsonSerializer.Deserialize<JwsHeader>(headerJson)!;
        var body = JsonSerializer.Deserialize<AsnDecodedPayload>(payloadJson)!;
        return (header, body);
    }
}
