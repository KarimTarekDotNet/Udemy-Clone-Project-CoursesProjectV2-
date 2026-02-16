using AutoMapper;
using UCourses_Back_End.Core.DTOs.RealTimeDTOs;
using UCourses_Back_End.Core.Entites.RealTime;

namespace UCourses_Back_End.Api.Mappings.Users
{
    public class ChatMappingProfile : Profile
    {
        public ChatMappingProfile()
        {
            CreateMap<Message, MessageDTO>();
        }
    }
}