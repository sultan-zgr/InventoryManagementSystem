namespace InventoryManagementSystem.Application.DTOs.User
{
    public class RegisterUserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } 
        public string EmailConfirmationToken { get; set; } 
    }


    public class LoginUserDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserDetailsDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } 
    }

    public class UpdateUserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
    public class UpdateUserRoleDTO
    {
        public string Email { get; set; }
        public string NewRole { get; set; }
    }
}
