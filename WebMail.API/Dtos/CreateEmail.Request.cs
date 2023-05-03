using System.ComponentModel.DataAnnotations;

namespace WebMail.API.Dtos
{
    public class CreateEmailRequest
    {
        [Required]
        public string Origin { get; set; }
        [Required]
        public string Destiny { get; set; }
        [Required]
        public string Subject { get; set; }
        public string? Body { get; set; }
    }
}
