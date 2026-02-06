using AutoMapper;
using TicketHub.API.DTOs.Category;
using TicketHub.API.DTOs.Event;
using TicketHub.API.DTOs.User;
using TicketHub.API.Models;

namespace TicketHub.API.Mappers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            /* User */
            CreateMap<RegisterDto, ApplicationUser>()
                .ForMember(user => user.UserName, config => config.MapFrom(dto => dto.Email));

            CreateMap<ApplicationUser, UserDto>();

            /* Category */
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryPutDto, Category>();

            /* Event */
            CreateMap<Event, EventDto>()
                .ForMember(x => x.CategoryName, config => config.MapFrom(x => x.Category!.Name));
            CreateMap<CategoryPutDto, Category>();
        }
    }
}