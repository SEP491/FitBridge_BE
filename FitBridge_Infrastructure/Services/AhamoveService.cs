using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FitBridge_Application.Configurations;
using FitBridge_Application.Dtos.Shippings;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FitBridge_Infrastructure.Services;

public class AhamoveService : IAhamoveService
{
    private readonly HttpClient _httpClient;
    private readonly AhamoveSettings _ahamoveSettings;
    private readonly ILogger<AhamoveService> _logger;
    private string? _cachedToken;

    public AhamoveService(
        HttpClient httpClient,
        IOptions<AhamoveSettings> ahamoveSettings,
        ILogger<AhamoveService> logger)
    {
        _httpClient = httpClient;
        _ahamoveSettings = ahamoveSettings.Value;
        _logger = logger;
        
        _httpClient.BaseAddress = new Uri(_ahamoveSettings.BaseUrl);
    }

    public async Task<string> GetTokenAsync()
    {
        if (!string.IsNullOrEmpty(_cachedToken))
        {
            return _cachedToken;
        }

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/v1/partner/register_account");
            request.Headers.Add("cache-control", "no-cache");
            
            var payload = new
            {
                mobile = "0123456789",
                name = "FitBridge",
                api_key = _ahamoveSettings.ApiKey
            };

            var jsonContent = JsonSerializer.Serialize(payload);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to get Ahamove token. Status: {response.StatusCode}, Response: {responseContent}");
                throw new BusinessException($"Failed to authenticate with Ahamove: {responseContent}");
            }

            var tokenResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            _cachedToken = tokenResponse.GetProperty("token").GetString();
            
            return _cachedToken!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Ahamove token");
            throw;
        }
    }

    public async Task<AhamoveOrderResponseDto> CreateOrderAsync(AhamoveCreateOrderDto request)
    {
        try
        {
            var token = await GetTokenAsync();

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/v3/orders");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpRequest.Headers.Add("cache-control", "no-cache");

            var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            httpRequest.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _logger.LogInformation($"Creating Ahamove order with payload: {jsonContent}");

            var response = await _httpClient.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to create Ahamove order. Status: {response.StatusCode}, Response: {responseContent}");
                throw new BusinessException($"Failed to create Ahamove order: {responseContent}");
            }

            _logger.LogInformation($"Ahamove order created successfully: {responseContent}");

            var orderResponse = JsonSerializer.Deserialize<AhamoveOrderResponseDto>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (orderResponse == null)
            {
                throw new BusinessException("Failed to deserialize Ahamove order response");
            }

            return orderResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Ahamove order");
            throw;
        }
    }
}

