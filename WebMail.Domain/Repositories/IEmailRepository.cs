using WebMail.Domain.Entities;

namespace WebMail.Domain.Repositories;

public interface IEmailRepository
{
    Task<int> CreateAsync(Email email);
    Task<Email?> GetByIdAsync(int id);
    Task<IEnumerable<Email>> GetAllAsync();
    Task<IEnumerable<Email>> GetAllNotSendedAsync();
    Task<bool> IncrementAttemptsAsync(int id);
    Task<bool> MarkAsSentAsync(int id);
}