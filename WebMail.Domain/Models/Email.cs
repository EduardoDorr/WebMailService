namespace WebMail.Domain.Models
{
    public class Email : BaseEntity
    {
        public string Origin { get; set; }
        public string Destiny { get; set; }
        public string Subject { get; set; }
        public string? Body { get; set; }
        public string? Attachment { get; set; }
        public DateTime GenerationDate { get; set; }
        public DateTime SendDate { get; set; }
    }
}
