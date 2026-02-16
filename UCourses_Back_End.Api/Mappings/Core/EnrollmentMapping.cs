using AutoMapper;
using UCourses_Back_End.Core.DTOs.CoreDTOs;
using UCourses_Back_End.Core.Entites.CoreModels;

namespace UCourses_Back_End.Api.Mappings.Core
{
    public class EnrollmentMapping : Profile
    {
        public EnrollmentMapping()
        {
            CreateMap<CreateEnrollmentDTO, Enrollment>()
                .ForMember(dest => dest.CourseId, opt => opt.Ignore())
                .ForMember(dest => dest.StudentId, opt => opt.Ignore())
                .ForMember(dest => dest.Course, opt => opt.Ignore())
                .ForMember(dest => dest.Student, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PublicId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<Enrollment, EnrollmentDTO>()
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => 
                    src.Course != null ? src.Course.Name : "Unknown"))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => 
                    src.Student != null && src.Student.AppUser != null 
                        ? src.Student.AppUser.FirstName + " " + src.Student.AppUser.LastName 
                        : "Unknown"))
                .ForMember(dest => dest.EnrolledAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<Enrollment, EnrollmentDetailsDTO>()
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => 
                    src.Course != null ? src.Course.PublicId : string.Empty))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => 
                    src.Course != null ? src.Course.Name : "Unknown"))
                .ForMember(dest => dest.CoursePrice, opt => opt.MapFrom(src => 
                    src.Course != null ? src.Course.Price : 0))
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => 
                    src.Student != null ? src.Student.PublicId : string.Empty))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => 
                    src.Student != null && src.Student.AppUser != null 
                        ? src.Student.AppUser.FirstName + " " + src.Student.AppUser.LastName 
                        : "Unknown"))
                .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src => 
                    src.Student != null && src.Student.AppUser != null 
                        ? src.Student.AppUser.Email 
                        : string.Empty))
                .ForMember(dest => dest.EnrolledAt, opt => opt.MapFrom(src => src.CreatedAt));
        }
    }
}
