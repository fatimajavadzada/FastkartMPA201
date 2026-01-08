namespace FastkartMPA201.Abstraction;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string message);
}
