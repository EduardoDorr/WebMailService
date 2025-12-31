using System.Diagnostics;

using WebMail.API.Interfaces;

namespace WebMail.API.Services;

public class EmailJobService : BackgroundService
{
    private readonly ILogger<EmailJobService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly int _timeDelay;

    public EmailJobService(
        IConfiguration configuration,
        IServiceProvider serviceProvider,
        ILogger<EmailJobService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _timeDelay = configuration.GetValue<int>("SendEmailService:TimeDelay");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var stopwatch = new Stopwatch();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Iniciando envio de e-mails");

                stopwatch.Start();

                await SendEmailsAsync(stoppingToken);

                stopwatch.Stop();

                _logger.LogInformation("Encerrando envio de e-mails em {Elapsed} ms", stopwatch.ElapsedMilliseconds);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado no serviço de envio de e-mails");
            }

            await Task.Delay(_timeDelay, stoppingToken);
        }
    }

    public async Task SendEmailsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        var sendService = scope.ServiceProvider.GetRequiredService<ISendEmailService>();

        await sendService.SendEmailsAsync(cancellationToken);
    }
}