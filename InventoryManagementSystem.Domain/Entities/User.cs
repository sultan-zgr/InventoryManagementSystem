using InventoryManagementSystem.Domain.Enums;

namespace InventoryManagementSystem.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; } // Şifre hash olarak saklanır
        public UserRole Role { get; set; } // Admin, Manager, Viewer
        public bool IsEmailConfirmed { get; set; } = false; // Varsayılan olarak doğrulanmamış
        public string EmailConfirmationToken { get; set; } // Doğrulama tokeni
        public DateTime? TokenCreatedAt { get; set; } // Token oluşturulma tarihi
    }
}
