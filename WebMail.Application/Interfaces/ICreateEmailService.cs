using WebMail.Application.Dtos;

namespace WebMail.Application.Interfaces;

public interface ICreateEmailService
{
    Task<GetEmailResponse> GetEmailById(int id);
    Task<CreateEmailResponse> CreateEmail(CreateEmailRequest emailRequest);
}