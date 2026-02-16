using AutoMapper;
using UCourses_Back_End.Core.DTOs.CoreDTOs;
using UCourses_Back_End.Core.Entites.CoreModels;

namespace UCourses_Back_End.Api.Mappings.Core
{
    public class CourseMapping : Profile
    {
        public CourseMapping()
        {
            CreateMap<CreateCourseDTO, Course>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.InstructorId, opt => opt.Ignore())
                .ForMember(dest => dest.DepartmentId, opt => opt.Ignore())
                .ForMember(dest => dest.Instructor, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.Enrollments, opt => opt.Ignore())
                .ForMember(dest => dest.Sections, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PublicId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<UpdateCourseDTO, Course>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.InstructorId, opt => opt.Ignore())
                .ForMember(dest => dest.DepartmentId, opt => opt.Ignore())
                .ForMember(dest => dest.Instructor, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.Enrollments, opt => opt.Ignore())
                .ForMember(dest => dest.Sections, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PublicId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<Course, CourseDTO>()
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => 
                    src.Instructor != null && src.Instructor.AppUser != null 
                        ? src.Instructor.AppUser.FirstName + " " + src.Instructor.AppUser.LastName 
                        : "Unknown"))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => 
                    src.Department != null ? src.Department.Name : "Unknown"));

            CreateMap<Course, CourseDetailsDTO>()
                .ForMember(dest => dest.InstructorId, opt => opt.MapFrom(src => 
                    src.Instructor != null ? src.Instructor.PublicId : string.Empty))
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => 
                    src.Instructor != null && src.Instructor.AppUser != null 
                        ? src.Instructor.AppUser.FirstName + " " + src.Instructor.AppUser.LastName 
                        : "Unknown"))
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => 
                    src.Department != null ? src.Department.PublicId : string.Empty))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => 
                    src.Department != null ? src.Department.Name : "Unknown"))
                .ForMember(dest => dest.EnrollmentsCount, opt => opt.MapFrom(src => 
                    src.Enrollments != null ? src.Enrollments.Count : 0))
                .ForMember(dest => dest.SectionsCount, opt => opt.MapFrom(src => 
                    src.Sections != null ? src.Sections.Count : 0));
        }
    }
}
