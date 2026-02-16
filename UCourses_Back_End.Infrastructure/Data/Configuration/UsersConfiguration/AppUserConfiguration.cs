using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCourses_Back_End.Core.Entites.AuthModel;
using UCourses_Back_End.Infrastructure.Data.Seeding;

namespace UCourses_Back_End.Infrastructure.Data.Configuration.UsersConfiguration
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasData(
                // Admin User
                new AppUser
                {
                    Id = SeedData.AdminUserId,
                    UserName = SeedData.AdminUserName,
                    NormalizedUserName = SeedData.AdminUserName.ToUpper(),
                    Email = SeedData.AdminEmail,
                    NormalizedEmail = SeedData.AdminEmail.ToUpper(),
                    EmailConfirmed = true,
                    FirstName = SeedData.AdminFirstName,
                    LastName = SeedData.AdminLastName,
                    PasswordHash = SeedData.AdminPasswordHash,
                    SecurityStamp = SeedData.AdminSecurityStamp,
                    ConcurrencyStamp = SeedData.AdminConcurrencyStamp,
                    ProviderId = SeedData.AdminUserIdProvider
                },
                // Instructor User
                new AppUser
                {
                    Id = SeedData.InstructorUserId,
                    UserName = SeedData.InstructorUserName,
                    NormalizedUserName = SeedData.InstructorUserName.ToUpper(),
                    Email = SeedData.InstructorEmail,
                    NormalizedEmail = SeedData.InstructorEmail.ToUpper(),
                    EmailConfirmed = true,
                    FirstName = SeedData.InstructorFirstName,
                    LastName = SeedData.InstructorLastName,
                    PasswordHash = SeedData.InstructorPasswordHash,
                    SecurityStamp = SeedData.InstructorSecurityStamp,
                    ConcurrencyStamp = SeedData.InstructorConcurrencyStamp,
                    ProviderId = SeedData.InstructorUserIdProvider
                },
                // Student User
                new AppUser
                {
                    Id = SeedData.StudentUserId,
                    UserName = SeedData.StudentUserName,
                    NormalizedUserName = SeedData.StudentUserName.ToUpper(),
                    Email = SeedData.StudentEmail,
                    NormalizedEmail = SeedData.StudentEmail.ToUpper(),
                    EmailConfirmed = true,
                    FirstName = SeedData.StudentFirstName,
                    LastName = SeedData.StudentLastName,
                    PasswordHash = SeedData.StudentPasswordHash,
                    SecurityStamp = SeedData.StudentSecurityStamp,
                    ConcurrencyStamp = SeedData.StudentConcurrencyStamp,
                    ProviderId = SeedData.StudentUserIdProvider
                }
            );
        }
    }
}