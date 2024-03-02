namespace WebMail.API.Dtos;

public sealed record CreateEmailRequest(string Origin, string Destiny, string Subject, string? Body, string? Attachment);