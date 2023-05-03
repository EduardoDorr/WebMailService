using AutoMapper;
using WebMail.API.Dtos;
using WebMail.API.Interfaces;
using WebMail.Domain.Models;
using WebMail.Infrastructure.Interfaces;

namespace WebMail.API.Services
{
    public class CreateEmailService : ICreateEmailService
    {
        private readonly ILogger<CreateEmailService> _logger;
        private readonly IEmailRepository _repository;
        private readonly IMapper _mapper;

        public CreateEmailService(IEmailRepository repository, IMapper mapper, ILogger<CreateEmailService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CreateEmailResponse> CreateEmail(CreateEmailRequest createEmailRequest)
        {
            _logger.LogInformation("Requisição para criação de um e-mail", createEmailRequest);
            var email = _mapper.Map<Email>(createEmailRequest);
            email.GenerationDate = DateTime.Now;

            var id = await _repository.CreateEmail(email);

            return new CreateEmailResponse() { Id = id, GenerationDate = email.GenerationDate };
        }

        public async Task<GetEmailResponse> GetEmailById(int id)
        {
            _logger.LogInformation("Requisição para buscar um e-mail de id {id}", id);
            var email = await _repository.GetEmailById(id);

            var result = _mapper.Map<GetEmailResponse>(email);

            return result;
        }
    }
}
