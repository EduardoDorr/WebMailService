using System.Collections.Concurrent;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

using WebMail.Domain.Repositories;
using WebMail.Application.Interfaces;
using WebMail.Application.Options;

namespace WebMail.Application.Services
{
    public class SendEmailService : BackgroundService, ISendEmailService
    {
        private readonly ILogger<SendEmailService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IEmailRepository _repository;
        private readonly ServiceOptions _serviceOptions;

        private readonly string _host;
        private readonly string _sender;
        private readonly string _password;
        private readonly int _port;
        private readonly int _timeDelay;

        public SendEmailService(IConfiguration configuration, IEmailRepository repository, IOptions<ServiceOptions> serviceOptions, ILogger<SendEmailService> logger)
        {
            _configuration = configuration;
            _repository = repository;
            _logger = logger;
            _serviceOptions = serviceOptions.Value;

            _port = _serviceOptions.Port;
            _host = _serviceOptions.Host;
            _sender = _serviceOptions.Sender;
            _password = _serviceOptions.Password;
            _timeDelay = _serviceOptions.TimeDelay;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Iniciando envio de e-mails");

                await SendEmails();

                _logger.LogInformation("Encerrando envio de e-mails");

                await Task.Delay(_timeDelay, stoppingToken);
            }
        }

        public async Task SendEmails()
        {
            var emails = (await _repository.GetEmailsNotSended()).ToList();

            _logger.LogDebug("Foram encontrados {count} e-mails", emails.Count);

            if (emails.Count == 0)
                return;

            var partitioner = Partitioner.Create(0, emails.Count);

            Parallel.ForEach(partitioner, async range =>
            {
                using var smtpClient = new SmtpClient();

                try
                {
                    smtpClient.Connect(_host, _port, SecureSocketOptions.StartTls);
                    smtpClient.Authenticate(_sender, _password);

                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        var mailMessage = CreateMessage(emails[i].Origin, emails[i].Destiny, emails[i].Subject, emails[i].Body, emails[i].Attachment);

                        try
                        {
                            await smtpClient.SendAsync(mailMessage);
                            await _repository.UpdateEmail(emails[i].Id);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Erro durante o envio de e-mail", mailMessage);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro durante o envio de e-mail", smtpClient);
                }
                finally
                {
                    smtpClient.Disconnect(true);
                }
            });
        }

        private MimeMessage CreateMessage(string origin, string toMail, string subject, string? body, string? attachment = null)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("EmailService", _sender));
            mailMessage.To.Add(new MailboxAddress("Destiny", toMail));
            mailMessage.Subject = $"{origin} - {subject}";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;

            if (attachment is not null)
                bodyBuilder.Attachments.Add(attachment);

            mailMessage.Body = bodyBuilder.ToMessageBody();

            return mailMessage;
        }
    }
}
