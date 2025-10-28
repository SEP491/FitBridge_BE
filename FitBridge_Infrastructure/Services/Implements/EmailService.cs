using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos.Emails;
using FitBridge_Application.Helpers;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Infrastructure.Jobs;
using FluentEmail.Core;
using FluentEmail.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Quartz;
using System;
using System.Text.Json;
using FitBridge_Application.Configurations;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;


namespace FitBridge_Infrastructure.Services.Implements;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly EmailSettings _emailSettings;
    private readonly ISchedulerFactory _schedulerFactory;
    public EmailService(IConfiguration configuration, ISchedulerFactory schedulerFactory, IOptions<EmailSettings> emailSettings)
    {
        _configuration = configuration;
        _schedulerFactory = schedulerFactory;
        _emailSettings = emailSettings.Value;
    }
    public async Task SendEmailAsync(string email, string subject, string message)
    {

        var messageModel = new MimeMessage();
        messageModel.Subject = subject;
        messageModel.From.Add(new MailboxAddress("FitBridge", _emailSettings.From));
        messageModel.To.Add(new MailboxAddress("User", email));
        messageModel.Body = new TextPart("html") { Text = message };

        using (var client = new SmtpClient())
        {
            client.Connect(_emailSettings.Host, _emailSettings.Port, false);
            client.Authenticate(_emailSettings.Username, _emailSettings.Password);
            var result = client.Send(messageModel);
            Console.WriteLine($"Email sent: {result}");
            client.Disconnect(true);
        }
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
            .UsingJobData("currentRetry", 0.ToString())
            .Build();
        var trigger = TriggerBuilder.Create()
        .WithIdentity($"send-email-trigger-{emailData.UserId}", "email-jobs")
        .StartNow() //Excecute immediately
        .Build();

        await scheduler.ScheduleJob(job, trigger);
    }
}
