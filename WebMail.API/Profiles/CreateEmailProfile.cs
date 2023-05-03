using AutoMapper;
using WebMail.API.Dtos;
using WebMail.Domain.Models;

namespace WebMail.API.Profiles
{
    public class CreateEmailProfile : Profile
    {
        public CreateEmailProfile()
        {
            CreateMap<CreateEmailRequest, Email>();
            CreateMap<Email, CreateEmailResponse>();
        }
    }
}
