namespace FitBridge_Application.Interfaces.Services;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string body);
    Task SendRegistrationConfirmationEmailAsync(string email, string confirmationLink, string fullName);
    Task SendAccountInformationEmailAsync(string email, string password);
}
