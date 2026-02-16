using AutoMapper;
using UCourses_Back_End.Core.DTOs.DashboardDTOs;
using UCourses_Back_End.Core.Entites.CoreModels;

namespace UCourses_Back_End.Api.Mappings.Users
{
    public class AdminDashboardMapping : Profile
    {
        public AdminDashboardMapping()
        {
            CreateMap<Enrollment, RecentActivityDTO>()
                .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => "Enrollment"))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => 
                    $"{src.Student.AppUser.FirstName} {src.Student.AppUser.LastName} enrolled in {src.Course.Name}"))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Student.PublicId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => 
                    $"{src.Student.AppUser.FirstName} {src.Student.AppUser.LastName}"));

            CreateMap<Course, RecentActivityDTO>()
                .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => "Course Created"))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => 
                    $"{src.Instructor.AppUser.FirstName} {src.Instructor.AppUser.LastName} created course: {src.Name}"))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Instructor.PublicId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => 
                    $"{src.Instructor.AppUser.FirstName} {src.Instructor.AppUser.LastName}"));
        }
    }
}
