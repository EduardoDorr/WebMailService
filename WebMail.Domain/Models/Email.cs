namespace WebMail.Domain.Models;

public class Email : BaseEntity
{
    public string Origin { get; set; } = null!;
    public string Destiny { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string? Body { get; set; }
    public string? Attachment { get; set; }
    public DateTime GenerationDate { get; set; } = DateTime.UtcNow;
    public DateTime? SendDate { get; set; } = null;
    public bool Sent { get; set; } = false;
    public int Retries { get; set; } = 0;
    public DateTime? LastAttemptDate { get; set; } = null;
}