using WebMail.Domain.Entities;

namespace WebMail.Domain.Repositories
{
    public interface IEmailRepository
    {
        Task<Email?> GetEmailById(int id);
        Task<IEnumerable<Email>> GetEmails();
        Task<IEnumerable<Email>> GetEmailsNotSended();
        Task<int> CreateEmail(Email email);
        Task<bool> UpdateEmail(int id);
    }
}