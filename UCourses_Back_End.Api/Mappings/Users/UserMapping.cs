using AutoMapper;
using UCourses_Back_End.Core.DTOs.AuthDTOs;
using UCourses_Back_End.Core.Entites.AuthModel;

namespace UCourses_Back_End.Api.Mappings.Users
{
    public class UserMapping : Profile
    {
        public UserMapping() 
        {
            CreateMap<RegisterDTO, AppUser>().ReverseMap();
        }
    }
}
