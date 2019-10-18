using System.Threading.Tasks;

namespace dotnet_backgroundjobs.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
