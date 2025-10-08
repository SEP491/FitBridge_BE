using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos.Emails;
using FitBridge_Application.Helpers;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Infrastructure.Jobs;
using FluentEmail.Core;
using FluentEmail.Smtp;
using Microsoft.Extensions.Configuration;
using Quartz;
using System;
using System.Net;
using System.Net.Mail;
using System.Text.Json;


namespace FitBridge_Infrastructure.Services.Implements;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ISchedulerFactory _schedulerFactory;
    public EmailService(IConfiguration configuration, ISchedulerFactory schedulerFactory)
    {
        _configuration = configuration;
        _schedulerFactory = schedulerFactory;
    }
    public async Task SendEmailAsync(string email, string subject, string message)
    {

        var smtpClient = new SmtpClient(_configuration["Smtp:Host"])
        {
            Port = int.Parse(_configuration["Smtp:Port"]),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            EnableSsl = true,
            Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"])
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["Smtp:From"]),
            Subject = subject,
            Body = message,
            IsBodyHtml = true,
        };
        mailMessage.To.Add(email);

        await smtpClient.SendMailAsync(mailMessage);
    }

    public async Task SendRegistrationConfirmationEmailAsync(string email, string confirmationLink, string fullName)
    {
        string subject = "Confirm your email";
        string message = EmailContentBuilder.BuildRegistrationConfirmationEmail(confirmationLink, fullName);
        await SendEmailAsync(email, subject, message);
    }
    
    public async Task SendAccountInformationEmailAsync(ApplicationUser user, string password, string role)
    {
        string subject = "Account Information";
        string message = EmailContentBuilder.BuildPtInformationEmail(user, password, role);

        if (role == ProjectConstant.UserRoles.GymOwner)
        {
            message = EmailContentBuilder.BuildGymOwnerInformationEmail(user, password, _configuration["FrontendUrl"]);
        }

        await SendEmailAsync(user.Email, subject, message);
    }

    public async Task ScheduleEmailAsync(AccountInformationEmailData emailData)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobData = JsonSerializer.Serialize(emailData);
        var job = JobBuilder.Create<SendAccountInformationEmailJob>()
            .WithIdentity($"send-email-{emailData.UserId}", "email-jobs")
            .UsingJobData("emailData", jobData)
            .UsingJobData("currentRetry", 0)
            .Build();
        var trigger = TriggerBuilder.Create()
        .WithIdentity($"send-email-trigger-{emailData.UserId}", "email-jobs")
        .StartNow() //Excecute immediately
        .Build();

        await scheduler.ScheduleJob(job, trigger);
    }
}
