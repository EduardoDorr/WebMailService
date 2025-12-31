using System.Diagnostics;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using WebMail.Application.Interfaces;
using WebMail.Application.Options;

namespace WebMail.Application.Services;

public class EmailJobService : BackgroundService
{
    private readonly ILogger<EmailJobService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly int _timeDelay;

    public EmailJobService(
        IOptions<ServiceOptions> options,
        IServiceProvider serviceProvider,
        ILogger<EmailJobService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _timeDelay = options.Value.TimeDelay;
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