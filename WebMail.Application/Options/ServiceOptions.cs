namespace WebMail.Application.Options;

public sealed record ServiceOptions(
    int Port,
    string Host,
    string Sender,
    string Password,
    int TimeDelay = 60000
);