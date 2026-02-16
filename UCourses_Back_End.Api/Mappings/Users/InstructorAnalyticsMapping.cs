using AutoMapper;
using UCourses_Back_End.Core.DTOs.DashboardDTOs;
using UCourses_Back_End.Core.Entites.CoreModels;

namespace UCourses_Back_End.Api.Mappings.Users
{
    public class InstructorAnalyticsMapping : Profile
    {
        public InstructorAnalyticsMapping()
        {
            CreateMap<Course, CourseEarningDTO>()
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.PublicId))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CoursePrice, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.EnrolledStudents, opt => opt.MapFrom(src => src.Enrollments.Count))
                .ForMember(dest => dest.TotalRevenue, opt => opt.MapFrom(src => src.Price * src.Enrollments.Count));

            CreateMap<Enrollment, StudentProgressSummary>()
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.Student.PublicId))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => 
                    $"{src.Student.AppUser.FirstName} {src.Student.AppUser.LastName}"))
                .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src => src.Student.AppUser.Email))
                .ForMember(dest => dest.EnrolledAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.CompletedSections, opt => opt.MapFrom(src => src.ProgressRecords.Count(p => p.IsCompleted)))
                .ForMember(dest => dest.TotalSections, opt => opt.MapFrom(src => src.ProgressRecords.Count))
                .ForMember(dest => dest.ProgressPercentage, opt => opt.MapFrom(src =>
                    src.ProgressRecords.Count > 0
                        ? Math.Round((decimal)src.ProgressRecords.Count(p => p.IsCompleted) / src.ProgressRecords.Count * 100, 2)
                        : 0));
        }
    }
}
