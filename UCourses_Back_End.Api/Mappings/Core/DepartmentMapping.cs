using AutoMapper;
using UCourses_Back_End.Core.DTOs.CoreDTOs;
using UCourses_Back_End.Core.Entites.CoreModels;

namespace UCourses_Back_End.Api.Mappings.Core
{
    public class DepartmentMapping : Profile
    {
        public DepartmentMapping()
        {
            CreateMap<CreateDepartmentDTO, Department>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Courses, opt => opt.Ignore())
                .ForMember(dest => dest.Instructors, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PublicId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<UpdateDepartmentDTO, Department>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Courses, opt => opt.Ignore())
                .ForMember(dest => dest.Instructors, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PublicId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<Department, DepartmentDTO>();

            CreateMap<Department, DepartmentDetailsDTO>()
                .ForMember(dest => dest.CoursesCount, opt => opt.MapFrom(src => src.Courses != null ? src.Courses.Count : 0))
                .ForMember(dest => dest.InstructorsCount, opt => opt.MapFrom(src => src.Instructors != null ? src.Instructors.Count : 0));
        }
    }
}
