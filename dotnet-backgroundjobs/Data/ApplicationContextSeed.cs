using System.Linq;
using System.Threading.Tasks;
using dotnet_backgroundjobs.Models;
using Microsoft.AspNetCore.Identity;

namespace dotnet_backgroundjobs.Data
{
    public class ApplicationContextSeed
    {
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher = new PasswordHasher<ApplicationUser>();

        public async Task SeedAsync(ApplicationDbContext context)
        {
            if (!context.ApplicationUsers.Any())
            {
                ApplicationUser user = InitUserDefault();
                await context.ApplicationUsers.AddAsync(user);
                await context.SaveChangesAsync();
            }
        }

        private ApplicationUser InitUserDefault()
        {
            ApplicationUser user= new ApplicationUser
            {
                Email = "testzinnia@gmail.com",
                NormalizedEmail = "testzinnia@gmail.com".ToUpper(),
                NormalizedUserName = "testingzinnia".ToUpper(),
                Name = "Test",
                UserName = "testingzinnia",
                LastName = "Zinnia",
                City = "Da Nang"
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, "Pass@word1");
            return user;
        }
    }
}
