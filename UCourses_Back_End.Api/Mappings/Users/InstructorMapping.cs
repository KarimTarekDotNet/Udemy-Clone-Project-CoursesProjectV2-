using AutoMapper;
using UCourses_Back_End.Core.DTOs.DashboardDTOs;
using UCourses_Back_End.Core.Entites.Users;

namespace UCourses_Back_End.Api.Mappings.Users
{
    public class InstructorMapping : Profile
    {
        public InstructorMapping()
        {
            CreateMap<Instructor, InstructorDTO>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.FirstName : "Unknown"))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.LastName : "Unknown"))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.Email : "Unknown"))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.PhoneNumber : null))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.ImageUrl : null))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null))
                .ForMember(dest => dest.CoursesCount, opt => opt.MapFrom(src => src.Courses != null ? src.Courses.Count : 0))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => DateOnly.FromDateTime(DateTime.UtcNow) <= src.EndContract));

            CreateMap<Instructor, InstructorDetailsDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.AppUserId))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.FirstName : "Unknown"))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.LastName : "Unknown"))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.Email : "Unknown"))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.PhoneNumber : null))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.AppUser != null ? src.AppUser.ImageUrl : null))
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.Department != null ? src.Department.PublicId : null))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null))
                .ForMember(dest => dest.CoursesCount, opt => opt.MapFrom(src => src.Courses != null ? src.Courses.Count : 0))
                .ForMember(dest => dest.StudentsCount, opt => opt.MapFrom(src => 
                    src.Courses != null ? src.Courses.SelectMany(c => c.Enrollments).Select(e => e.StudentId).Distinct().Count() : 0))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => DateOnly.FromDateTime(DateTime.UtcNow) <= src.EndContract));
        }
    }
}
