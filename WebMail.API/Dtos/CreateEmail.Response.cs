using WebMail.Domain.Models;

namespace WebMail.API.Dtos;

public sealed class CreateEmailResponse : BaseEntity
{
    public DateTime GenerationDate { get; set; }
}