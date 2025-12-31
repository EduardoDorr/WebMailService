using AutoMapper;

using MailKit.Net.Smtp;
using MailKit.Security;

using MimeKit;

using WebMail.API.Dtos;
using WebMail.API.Interfaces;
using WebMail.Domain.Interfaces;
using WebMail.Domain.Models;

namespace WebMail.API.Services;

public class EmailService : ICreateEmailService, ISendEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IEmailRepository _repository;
    private readonly IMapper _mapper;

    private readonly string _host;
    private readonly string _sender;
    private readonly string _password;
    private readonly int _port;

    public EmailService(
        IEmailRepository repository,
        IMapper mapper,
        IConfiguration configuration,
        ILogger<EmailService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;

        _port = configuration.GetValue<int>("EmailSettings:Port");
        _host = configuration.GetValue<string>("EmailSettings:Host");
        _sender = configuration.GetValue<string>("EmailSettings:Sender");
        _password = configuration.GetValue<string>("EmailSettings:Password");
    }

    public async Task<CreateEmailResponse> CreateEmailAsync(CreateEmailRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Requisição para criação de um e-mail: {EmailRequest}", request);

        var email = _mapper.Map<Email>(request);

        var id = await _repository.CreateAsync(email);

        _ = Task.Run(() => SendEmailAsync(email, CancellationToken.None));

        return new CreateEmailResponse() { Id = id, GenerationDate = email.GenerationDate };
    }

    public async Task<GetEmailResponse> GetEmailByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Requisição para buscar um e-mail de id {Id}", id);

        var email = await _repository.GetByIdAsync(id);

        var result = _mapper.Map<GetEmailResponse>(email);

        return result;
    }

    public async Task<bool> SendEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await HandleEmailAsync(
            email,
            TrySendAsync,
            cancellationToken);
    }

    public async Task SendEmailsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Busca por e-mails não enviados");

        var emails = (await _repository.GetAllNotSendedAsync()).ToList();

        _logger.LogDebug("Foram encontrados {Count} e-mails", emails.Count);

        if (emails.Count == 0)
            return;

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = 4,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(
            emails,
            options,
            async (email, ct) =>
            {
                await HandleEmailAsync(
                    email,
                    TrySendAsync,
                    ct);
            });
    }

    private async Task<bool> HandleEmailAsync(Email email, Func<Email, SmtpClient, CancellationToken, Task<bool>> executeAsync, CancellationToken cancellationToken = default)
    {
        using var smtpClient = new SmtpClient();

        try
        {
            await smtpClient.ConnectAsync(_host, _port, SecureSocketOptions.StartTls, cancellationToken);
            await smtpClient.AuthenticateAsync(_sender, _password, cancellationToken);

            var sent = await executeAsync(email, smtpClient, cancellationToken);

            if (sent)
                await _repository.MarkAsSentAsync(email.Id);
            else
                await _repository.IncrementAttemptsAsync(email.Id);

            return sent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o envio de e-mail");
        }
        finally
        {
            await smtpClient.DisconnectAsync(true, cancellationToken);
        }

        return false;
    }

    private async Task<bool> TrySendAsync(Email email, SmtpClient smtpClient, CancellationToken cancellationToken = default)
    {
        var mailMessage = CreateMessage(
            email.Origin,
            email.Destiny,
            email.Subject,
            email.Body,
            email.Attachment);

        try
        {
            await smtpClient.SendAsync(mailMessage, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao enviar email para {To} com assunto {Subject}",
                mailMessage.To.ToString(),
                mailMessage.Subject);
        }

        return false;
    }

    private MimeMessage CreateMessage(string origin, string toMail, string subject, string? body, string? attachment = null)
    {
        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress("EmailService", _sender));
        mailMessage.To.Add(new MailboxAddress("Destiny", toMail));
        mailMessage.Subject = $"{origin} - {subject}";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = body
        };

        if (attachment is not null)
            bodyBuilder.Attachments.Add(attachment);

        mailMessage.Body = bodyBuilder.ToMessageBody();

        return mailMessage;
    }
}