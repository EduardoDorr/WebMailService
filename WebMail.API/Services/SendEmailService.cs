using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Collections.Concurrent;
using WebMail.API.Interfaces;
using WebMail.Infrastructure.Interfaces;

namespace WebMail.API.Services
{
    public class SendEmailService : BackgroundService, ISendEmailService
    {
        private readonly ILogger<SendEmailService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IEmailRepository _repository;

        private readonly string _host;
        private readonly string _sender;
        private readonly string _password;
        private readonly int _port;
        private readonly int _timeDelay;

        public SendEmailService(IConfiguration configuration, IEmailRepository repository, ILogger<SendEmailService> logger)
        {
            _configuration = configuration;
            _repository = repository;
            _logger = logger;

            _port = _configuration.GetValue<int>("EmailSettings:Port");
            _host = _configuration.GetValue<string>("EmailSettings:Host");
            _sender = _configuration.GetValue<string>("EmailSettings:Sender");
            _password = _configuration.GetValue<string>("EmailSettings:Password");
            _timeDelay = _configuration.GetValue<int>("SendEmailService:TimeDelay");
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
            _logger.LogInformation("Foram encontrados {count} e-mails", emails.Count);

            if (!emails.Any())
                return;

            var partitioner = Partitioner.Create(0, emails.Count);

            Parallel.ForEach(partitioner, range =>
            {
                using var smtpClient = new SmtpClient();

                try
                {
                    smtpClient.Connect(_host, _port, SecureSocketOptions.StartTls);
                    smtpClient.Authenticate(_sender, _password);

                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        var mailMessage = CreateMessage(emails[i].Origin, emails[i].Destiny, emails[i].Subject, emails[i].Body);

                        try
                        {
                            smtpClient.Send(mailMessage);
                            _repository.UpdateEmail(emails[i].Id);
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

        private MimeMessage CreateMessage(string origin, string toMail, string subject, string body)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("EmailService", _sender));
            mailMessage.To.Add(new MailboxAddress("Destiny", toMail));
            mailMessage.Subject = $"{origin} - {subject}";
            mailMessage.Body = new TextPart(TextFormat.Html) { Text = body };

            return mailMessage;
        }
    }
}
