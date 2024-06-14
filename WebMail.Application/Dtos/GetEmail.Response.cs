namespace WebMail.Application.Dtos;

public sealed record GetEmailResponse(
    string Origin,
    string Destiny,
    string Subject,
    string? Body,
    DateTime GenerationDate,
    DateTime SendDate);