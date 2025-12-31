using WebMail.API.Dtos;

namespace WebMail.API.Interfaces;

public interface ICreateEmailService
{
    Task<CreateEmailResponse> CreateEmailAsync(CreateEmailRequest request, CancellationToken cancellationToken = default);
    Task<GetEmailResponse> GetEmailByIdAsync(int id, CancellationToken cancellationToken = default);
}