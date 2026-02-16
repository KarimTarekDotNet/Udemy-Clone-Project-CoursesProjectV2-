using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace UCourses_Back_End.Infrastructure.Data
{
    public class AppDbContextFactory : IDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext()
        {
            var option = new DbContextOptionsBuilder<ApplicationDbContext>();
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var connect = config.GetConnectionString("DefaultConnection");
            option.UseSqlServer(connect);
            return new ApplicationDbContext(option.Options);
        }
    }
}
