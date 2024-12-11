using InventoryManagementSystem.Application.DTOs.User;
using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Application.Interfaces
{
    public interface IUserService
    {
        Task RegisterUserAsync(RegisterUserDTO registerUserDto, string createdByRole);
        Task<UserDetailsDTO> GetUserByIdAsync(Guid id);
        Task<IEnumerable<UserDetailsDTO>> GetAllUsersAsync(int page = 1, int pageSize = 10);
        Task UpdateUserAsync(Guid id, UpdateUserDTO updateUserDto);
        Task DeleteUserAsync(Guid id);
        Task<string> LoginAsync(LoginUserDTO loginUserDto);
        Task<string> GenerateEmailConfirmationTokenAsync(Guid userId);
        Task ConfirmEmailAsync(string token);


    }
}
