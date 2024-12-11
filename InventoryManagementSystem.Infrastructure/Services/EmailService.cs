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
            // SMTP veya üçüncü taraf hizmetler (örneğin SendGrid) ile e-posta gönderimi
            await Task.CompletedTask; // Gerçek e-posta gönderimi burada yapılacak
        }
    }
}
