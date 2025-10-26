using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using FitBridge_Application.Dtos.Emails;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Exceptions;
using Quartz;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Application.Commons.Constants;

namespace FitBridge_Infrastructure.Jobs;

public class SendAccountInformationEmailJob(IEmailService _emailService, ILogger<SendAccountInformationEmailJob> _logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var maxRetries = ProjectConstant.MaxRetries;
        var currentRetry = context.JobDetail.JobDataMap.GetIntValue("currentRetry");
        var emailDataJson = context.JobDetail.JobDataMap.GetString("emailData") ?? throw new NotFoundException("Email data not found");

        var emailData = JsonSerializer.Deserialize<AccountInformationEmailData>(emailDataJson) ?? throw new NotFoundException("Email data not found");
        _logger.LogInformation("SendAccountInformationEmailJob started for user: {Email}, Role: {Role}",
            emailData.Email, emailData.Role);

        try
        {
            var user = new ApplicationUser
            {
                Id = emailData.UserId,
                Email = emailData.Email,
                FullName = emailData.FullName,
                PhoneNumber = emailData.PhoneNumber,
                GymName = emailData.GymName ?? "",
                TaxCode = emailData.TaxCode ?? ""
            };

            if (emailData.EmailType == ProjectConstant.EmailTypes.RegistrationConfirmationEmail)
            {
                await _emailService.SendRegistrationConfirmationEmailAsync(emailData.Email, emailData.ConfirmationLink, emailData.FullName);
            }
            else
            {
                await _emailService.SendAccountInformationEmailAsync(user, emailData.Password, emailData.Role);
            }
            _logger.LogInformation("Email sent successfully to: {Email}", emailData.Email);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send email to: {emailData.Email}, Role: {emailData.Role}, Retry: {currentRetry + 1}/{maxRetries}");
            if (currentRetry < maxRetries)
            {
                context.JobDetail.JobDataMap.Put("currentRetry", currentRetry + 1);
                var newTrigger = context.Trigger.GetTriggerBuilder()
                .StartAt(DateTimeOffset.UtcNow.AddSeconds(20))
                .Build();
                await context.Scheduler.RescheduleJob(context.Trigger.Key, newTrigger);

                _logger.LogInformation("Rescheduled email job for {Email} to retry in {Delay} seconds",
                    emailData.Email, 20);
            }
            else
            {
                _logger.LogCritical("Failed to send email after {MaxRetries} attempts for user: {Email}",
                    maxRetries, emailData.Email);
                throw new BusinessException($"Failed to send email after {maxRetries} attempts for user: {emailData.Email}");
            }
        }
    }
}
