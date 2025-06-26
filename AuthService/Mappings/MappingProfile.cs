using AutoMapper;
using AuthService.Application.Dtos.User;
using AuthService.Domain.Entities;
using AuthService.Application.Dtos.Auth;

namespace AuthService.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserReadDto>().ReverseMap() ;
            CreateMap<UserWriteDto, User>().ReverseMap();
            CreateMap<LoginRequest, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
                .ReverseMap();
            CreateMap<RegisterRequest, User>().ReverseMap();
            
        }
    }
}