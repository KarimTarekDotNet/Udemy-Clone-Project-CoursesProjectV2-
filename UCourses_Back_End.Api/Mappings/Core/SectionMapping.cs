using AutoMapper;
using UCourses_Back_End.Core.DTOs.CoreDTOs;
using UCourses_Back_End.Core.Entites.CoreModels;

namespace UCourses_Back_End.Api.Mappings.Core
{
    public class SectionMapping : Profile
    {
        public SectionMapping()
        {
            CreateMap<CreateSectionDTO, Section>()
                .ForMember(dest => dest.VideoUrl, opt => opt.Ignore())
                .ForMember(dest => dest.PdfUrl, opt => opt.Ignore())
                .ForMember(dest => dest.CourseId, opt => opt.Ignore())
                .ForMember(dest => dest.Course, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PublicId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<UpdateSectionDTO, Section>()
                .ForMember(dest => dest.VideoUrl, opt => opt.Ignore())
                .ForMember(dest => dest.PdfUrl, opt => opt.Ignore())
                .ForMember(dest => dest.CourseId, opt => opt.Ignore())
                .ForMember(dest => dest.Course, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PublicId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<Section, SectionDTO>()
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => 
                    src.Course != null ? src.Course.Name : "Unknown"));

            CreateMap<Section, SectionDetailsDTO>()
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => 
                    src.Course != null ? src.Course.PublicId : string.Empty))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => 
                    src.Course != null ? src.Course.Name : "Unknown"));
        }
    }
}
