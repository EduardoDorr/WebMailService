using WebMail.Domain.Models;

namespace WebMail.Infrastructure.Interfaces
{
    public interface IEmailRepository
    {
        Task<int> CreateEmail(Email email);
        Task<Email> GetEmailById(int id);
        Task<IEnumerable<Email>> GetEmails();
        Task<IEnumerable<Email>> GetEmailsNotSended();
        Task<bool> UpdateEmail(int id);
    }
}
