using System.Threading.Tasks;

namespace InventoryManagementSystem.Infrastructure.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }

    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            await Task.CompletedTask; //TODO: E-Posta gönderimi burada yapılacak
        }
    }
}
