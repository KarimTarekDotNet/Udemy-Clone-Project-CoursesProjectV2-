using AutoMapper;
using UCourses_Back_End.Core.DTOs.DashboardDTOs;
using UCourses_Back_End.Core.Entites.CoreModels;

namespace UCourses_Back_End.Api.Mappings.Core
{
    public class CourseProgressMapping : Profile
    {
        public CourseProgressMapping()
        {
            CreateMap<CourseProgress, SectionProgressDTO>()
                .ForMember(dest => dest.SectionId, opt => opt.MapFrom(src => src.Section.PublicId))
                .ForMember(dest => dest.SectionName, opt => opt.MapFrom(src => src.Section.Name));
        }
    }
}
