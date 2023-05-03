using WebMail.API.Dtos;

namespace WebMail.API.Interfaces
{
    public interface ICreateEmailService
    {
        Task<CreateEmailResponse> CreateEmail(CreateEmailRequest emailRequest);
        Task<GetEmailResponse> GetEmailById(int id);
    }
}
