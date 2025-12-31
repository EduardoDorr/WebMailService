namespace WebMail.Application.Options;

public sealed class ServiceOptions
{
    public int Port { get; init; }
    public string Host { get; init; }
    public string Sender { get; init; }
    public string Password { get; init; }
    public int TimeDelay { get; init; } = 60000;
}