using WebMail.Domain.Models;

namespace WebMail.API.Interfaces;

public interface ISendEmailService
{
    Task<bool> SendEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task SendEmailsAsync(CancellationToken cancellationToken = default);
}