namespace UCourses_Back_End.Core.Constants
{
    public static class Roles
    {
        public const string Student = "Student";
        public const string Instructor = "Instructor";
        public const string Admin = "Admin";

        public static bool IsValid(string role)
        {
            return role == Student || role == Instructor || role == Admin;
        }

        public static string Normalize(string role)
        {
            if (role.Equals(Student, StringComparison.OrdinalIgnoreCase))
                return Student;
            if (role.Equals(Instructor, StringComparison.OrdinalIgnoreCase))
                return Instructor;
            if (role.Equals(Admin, StringComparison.OrdinalIgnoreCase))
                return Admin;
            
            return role;
        }
    }
}
