namespace UCourses_Back_End.Infrastructure.Data.Seeding
{
    public static class SeedData
    {
        // Roles
        public const string Admin = "Admin";
        public const string Instructor = "Instructor";
        public const string Student = "Student";

        // Admin Data
        public const string AdminEmail = "admin@ucourses.com";
        public const string AdminUserName = "system_admin";
        public const string AdminFirstName = "System";
        public const string AdminLastName = "Admin";
        public const string AdminPassword = "Admin@12345";
        public const string AdminPasswordHash = "AQAAAAIAAYagAAAAEApg0LLktwX8NyeBdwg0ZoL5ZlibZb2GXVgum6NUfBDGZNdVgna2PGAQSWGM6TtJdg=="; // Admin@12345
        public const string AdminUserId = "a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d";
        public const string AdminEntityId = "d4e5f6a7-b8c9-4d0e-1f2a-3b4c5d6e7f8a";
        public const string AdminPublicId = "ADM-d4e5f6a7";
        public const string AdminSecurityStamp = "ADMIN-SECURITY-STAMP-12345";
        public const string AdminConcurrencyStamp = "ADMIN-CONCURRENCY-12345";

        // Instructor Data
        public const string InstructorEmail = "instructor@ucourses.com";
        public const string InstructorUserName = "main_instructor";
        public const string InstructorFirstName = "Main";
        public const string InstructorLastName = "Instructor";
        public const string InstructorPassword = "Instructor@123";
        public const string InstructorPasswordHash = "AQAAAAIAAYagAAAAEGYkccUpGHTwfdA4FBPmLQAomDlnch/RmmjWZIAh5ABhvFC5y7lbrU7jLry5lCt/gQ=="; // Instructor@123
        public const string InstructorUserId = "b2c3d4e5-f6a7-4b8c-9d0e-1f2a3b4c5d6e";
        public const string InstructorEntityId = "e5f6a7b8-c9d0-4e1f-2a3b-4c5d6e7f8a9b";
        public const string InstructorPublicId = "INST-e5f6a7b8";
        public const string InstructorSecurityStamp = "INSTRUCTOR-SECURITY-STAMP-12345";
        public const string InstructorConcurrencyStamp = "INSTRUCTOR-CONCURRENCY-12345";

        // Student Data
        public const string StudentEmail = "student@ucourses.com";
        public const string StudentUserName = "main_student";
        public const string StudentFirstName = "Main";
        public const string StudentLastName = "Student";
        public const string StudentPassword = "Student@123";
        public const string StudentPasswordHash = "AQAAAAIAAYagAAAAEDcLX8o5AJxGucVj9ZHHkjDZIGzHb8L5tNc2As6sbuWkT1uL9PRGsgMf8VlGG7dN9Q=="; // Student@123
        public const string StudentUserId = "c3d4e5f6-a7b8-4c9d-0e1f-2a3b4c5d6e7f";
        public const string StudentEntityId = "f6a7b8c9-d0e1-4f2a-3b4c-5d6e7f8a9b0c";
        public const string StudentPublicId = "STUD-f6a7b8c9";
        public const string StudentSecurityStamp = "STUDENT-SECURITY-STAMP-12345";
        public const string StudentConcurrencyStamp = "STUDENT-CONCURRENCY-12345";

        public const string AdminUserIdProvider = "a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5f";
        public const string StudentUserIdProvider = "a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5p";
        public const string InstructorUserIdProvider = "a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5q";

        // Common Settings
        public const int ContractYears = 1;
        public const bool IsApproved = true;

        // Dates
        public static readonly DateOnly SeedDate = new(2024, 1, 1);
        public static readonly DateOnly InstructorEndContract = new(2025, 1, 1);

        // Department Data
        public const string DepartmentId1 = "a1a1a1a1-b2b2-4c3c-d4d4-e5e5e5e5e5e5";
        public const string DepartmentPublicId1 = "DEPT-a1a1a1a1";
        public const string DepartmentName1 = "Web Development";
        public const string DepartmentDescription1 = "Learn modern web development technologies";
        public const string DepartmentImageUrl1 = "/Images/Departments/web-dev.jpg";

        public const string DepartmentId2 = "b2b2b2b2-c3c3-4d4d-e5e5-f6f6f6f6f6f6";
        public const string DepartmentPublicId2 = "DEPT-b2b2b2b2";
        public const string DepartmentName2 = "Mobile Development";
        public const string DepartmentDescription2 = "Master mobile app development";
        public const string DepartmentImageUrl2 = "/Images/Departments/mobile-dev.jpg";

        // Course Data
        public const string CourseId1 = "c1c1c1c1-d2d2-4e3e-f4f4-a5a5a5a5a5a5";
        public const string CoursePublicId1 = "CRS-c1c1c1c1";
        public const string CourseName1 = "ASP.NET Core Complete Guide";
        public const string CourseDescription1 = "Complete guide to building web applications with ASP.NET Core";
        public const decimal CoursePrice1 = 499.99m;
        public const string CourseImageUrl1 = "/Images/Courses/aspnet-core.jpg";

        public const string CourseId2 = "d2d2d2d2-e3e3-4f4f-a5a5-b6b6b6b6b6b6";
        public const string CoursePublicId2 = "CRS-d2d2d2d2";
        public const string CourseName2 = "React Native Masterclass";
        public const string CourseDescription2 = "Build cross-platform mobile apps with React Native";
        public const decimal CoursePrice2 = 599.99m;
        public const string CourseImageUrl2 = "/Images/Courses/react-native.jpg";

        // Section Data
        public const string SectionId1 = "e1e1e1e1-f2f2-4a3a-b4b4-c5c5c5c5c5c5";
        public const string SectionPublicId1 = "SEC-e1e1e1e1";
        public const string SectionName1 = "Introduction to ASP.NET Core";
        public const string SectionDescription1 = "Getting started with ASP.NET Core framework";
        public const string SectionVideoUrl1 = "/Videos/Sections/aspnet-intro.mp4";
        public const string SectionPdfUrl1 = "/PDFs/Sections/aspnet-intro.pdf";

        public const string SectionId2 = "f2f2f2f2-a3a3-4b4b-c5c5-d6d6d6d6d6d6";
        public const string SectionPublicId2 = "SEC-f2f2f2f2";
        public const string SectionName2 = "React Native Basics";
        public const string SectionDescription2 = "Learn the fundamentals of React Native";
        public const string SectionVideoUrl2 = "/Videos/Sections/react-native-basics.mp4";

        // Enrollment Data
        public const string EnrollmentId1 = "a1a1a1a1-b2b2-4c3c-d4d4-111111111111";
        public const string EnrollmentPublicId1 = "ENRL-a1a1a1a1";

        public const string EnrollmentId2 = "b2b2b2b2-c3c3-4d4d-e5e5-222222222222";
        public const string EnrollmentPublicId2 = "ENRL-b2b2b2b2";

        // Payment Data
        public const int PaymentId1 = 1;
        public const decimal PaymentAmount1 = 499.99m;
        public const string PaymentMethod1 = "CreditCard";
        public const string PaymentTransactionId1 = "TXN-2024-001-ABC123";
        public static readonly DateTime PaymentDate1 = new(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        public const int PaymentId2 = 2;
        public const decimal PaymentAmount2 = 599.99m;
        public const string PaymentMethod2 = "PayPal";
        public const string PaymentTransactionId2 = "TXN-2024-002-XYZ789";
        public static readonly DateTime PaymentDate2 = new(2024, 1, 20, 14, 45, 0, DateTimeKind.Utc);
    }
}