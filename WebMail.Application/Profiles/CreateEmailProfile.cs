using AutoMapper;

using WebMail.Domain.Entities;
using WebMail.Application.Dtos;

namespace WebMail.Application.Profiles;

public class CreateEmailProfile : Profile
{
    public CreateEmailProfile()
    {
        CreateMap<CreateEmailRequest, Email>().ReverseMap();
    }
}