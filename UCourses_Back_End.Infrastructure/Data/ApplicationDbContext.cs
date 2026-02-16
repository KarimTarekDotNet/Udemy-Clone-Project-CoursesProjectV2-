using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using UCourses_Back_End.Core.Entites.AuthModel;
using UCourses_Back_End.Core.Entites.CoreModels;
using UCourses_Back_End.Core.Entites.RealTime;
using UCourses_Back_End.Core.Entites.Users;

namespace UCourses_Back_End.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<CourseProgress> CourseProgress { get; set; }
        public DbSet<Payment> Payments { get; set; }
        
        // RealTime
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.UseCollation("Arabic_CI_AI");
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            builder.Entity<Admin>(entity =>
            {
                entity.HasIndex(a => a.AppUserId).IsUnique();
                entity.HasOne(a => a.AppUser)
                      .WithOne()
                      .HasForeignKey<Admin>(a => a.AppUserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Instructor>(entity =>
            {
                entity.HasIndex(i => i.AppUserId).IsUnique();
                entity.HasOne(i => i.AppUser)
                      .WithOne()
                      .HasForeignKey<Instructor>(i => i.AppUserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Student>(entity =>
            {
                entity.HasIndex(s => s.AppUserId).IsUnique();
                entity.HasOne(s => s.AppUser)
                      .WithOne()
                      .HasForeignKey<Student>(s => s.AppUserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<AppUser>(entity =>
            {
                entity.HasMany(x => x.RefreshTokens)
                      .WithOne(x => x.AppUser)
                      .HasForeignKey(x => x.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Department Relations
            builder.Entity<Department>(entity =>
            {
                entity.HasMany(d => d.Courses)
                      .WithOne(c => c.Department)
                      .HasForeignKey("DepartmentId")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(d => d.Instructors)
                      .WithOne(i => i.Department)
                      .HasForeignKey("DepartmentId")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Course Relations
            builder.Entity<Course>(entity =>
            {
                entity.HasOne(c => c.Instructor)
                      .WithMany(i => i.Courses)
                      .HasForeignKey(c => c.InstructorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(c => c.Sections)
                      .WithOne(s => s.Course)
                      .HasForeignKey("CourseId")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.Enrollments)
                      .WithOne(e => e.Course)
                      .HasForeignKey(e => e.CourseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Enrollment Relations
            builder.Entity<Enrollment>(entity =>
            {
                entity.HasOne(e => e.Student)
                      .WithMany(s => s.Enrollments)
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.CourseId, e.StudentId }).IsUnique();

                entity.HasMany(e => e.ProgressRecords)
                      .WithOne(p => p.Enrollment)
                      .HasForeignKey(p => p.EnrollmentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Payment)
                      .WithOne(p => p.Enrollment)
                      .HasForeignKey<Payment>(p => p.EnrollmentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // CourseProgress Relations
            builder.Entity<CourseProgress>(entity =>
            {
                entity.HasOne(p => p.Section)
                      .WithMany()
                      .HasForeignKey(p => p.SectionId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(p => new { p.EnrollmentId, p.SectionId }).IsUnique();
            });

            // Payment Relations
            builder.Entity<Payment>(entity =>
            {
                entity.HasOne(p => p.Student)
                      .WithMany(s => s.Payments)
                      .HasForeignKey(p => p.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Enrollment)
                      .WithOne(e => e.Payment)
                      .HasForeignKey<Payment>(p => p.EnrollmentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(p => p.TransactionId).IsUnique();
                entity.HasIndex(p => p.EnrollmentId).IsUnique();
            });

            // Conversation Relations
            builder.Entity<Conversation>(entity =>
            {
                entity.HasMany(c => c.Participants)
                      .WithOne(p => p.Conversation)
                      .HasForeignKey(p => p.ConversationId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.Messages)
                      .WithOne(m => m.Conversation)
                      .HasForeignKey(m => m.ConversationId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ConversationParticipant Relations
            builder.Entity<ConversationParticipant>(entity =>
            {
                entity.HasKey(p => new { p.ConversationId, p.UserId });

                entity.HasOne(p => p.Conversation)
                      .WithMany(c => c.Participants)
                      .HasForeignKey(p => p.ConversationId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Message Relations
            builder.Entity<Message>(entity =>
            {
                entity.HasOne(m => m.Conversation)
                      .WithMany(c => c.Messages)
                      .HasForeignKey(m => m.ConversationId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(m => m.ConversationId);
                entity.HasIndex(m => m.SenderId);
                entity.HasIndex(m => m.SentAt);
            });
        }

    }
}