using Microsoft.Extensions.Logging;

using AutoMapper;

using WebMail.Domain.Entities;
using WebMail.Domain.Repositories;
using WebMail.Application.Dtos;
using WebMail.Application.Interfaces;

namespace WebMail.Application.Services;

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
        _logger.LogDebug("Requisição para criação de um e-mail", createEmailRequest);

        var email = _mapper.Map<Email>(createEmailRequest);
        email.GenerationDate = DateTime.Now;

        var id = await _repository.CreateEmail(email);

        return new CreateEmailResponse(id, email.GenerationDate);
    }

    public async Task<GetEmailResponse> GetEmailById(int id)
    {
        _logger.LogDebug("Requisição para buscar um e-mail de id {id}", id);

        var email = await _repository.GetEmailById(id);

        var result = _mapper.Map<GetEmailResponse>(email);

        return result;
    }
}