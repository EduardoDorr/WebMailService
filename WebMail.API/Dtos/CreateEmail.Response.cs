using WebMail.Domain.Models;

namespace WebMail.API.Dtos
{
    public class CreateEmailResponse : BaseEntity
    {
        public DateTime GenerationDate { get; set; }
    }
}
