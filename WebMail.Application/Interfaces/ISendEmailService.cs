using WebMail.Domain.Entities;

namespace WebMail.Application.Interfaces;

public interface ISendEmailService
{
    Task<bool> SendEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task SendEmailsAsync(CancellationToken cancellationToken = default);
}