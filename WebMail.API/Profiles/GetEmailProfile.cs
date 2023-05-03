using AutoMapper;
using WebMail.API.Dtos;
using WebMail.Domain.Models;

namespace WebMail.API.Profiles
{
    public class GetEmailProfile : Profile
    {
        public GetEmailProfile()
        {
            CreateMap<Email, GetEmailResponse>();
        }
    }
}
