using AutoMapper;
using TicketHub.API.DTOs;
using TicketHub.API.Models;

namespace TicketHub.API.Mappers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<RegisterDto, ApplicationUser>()
                .ForMember(user => user.UserName, config => config.MapFrom(dto => dto.Email));

            CreateMap<ApplicationUser, UserDto>();
        }
    }
}