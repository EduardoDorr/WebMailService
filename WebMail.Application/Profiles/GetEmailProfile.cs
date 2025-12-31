using AutoMapper;

using WebMail.Domain.Entities;
using WebMail.Application.Dtos;

namespace WebMail.Application.Profiles;

public class GetEmailProfile : Profile
{
    public GetEmailProfile()
    {
        CreateMap<Email, GetEmailResponse>().ReverseMap();
    }
}