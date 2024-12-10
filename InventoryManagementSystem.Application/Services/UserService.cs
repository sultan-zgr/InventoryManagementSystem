using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using InventoryManagementSystem.Application.DTOs.User;
using InventoryManagementSystem.Application.Interfaces;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace InventoryManagementSystem.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task RegisterUserAsync(RegisterUserDTO registerUserDto, string createdByRole)
        {
            // Sadece Admin kullanıcılar Viewer dışında bir rol atayabilir
            if (registerUserDto.Role != "Viewer" && createdByRole != "Admin")
                throw new UnauthorizedAccessException("Only Admins can assign roles other than Viewer.");

            // Kullanıcıyı Entity'ye mapleyin ve şifreyi hashleyin
            var userEntity = _mapper.Map<User>(registerUserDto);
            userEntity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password);

            // Kullanıcıyı kaydedin
            await _userRepository.AddAsync(userEntity);
        }

        public async Task<UserDetailsDTO> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new KeyNotFoundException("User not found.");

            return _mapper.Map<UserDetailsDTO>(user);
        }

        public async Task<IEnumerable<UserDetailsDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDetailsDTO>>(users);
        }

        public async Task UpdateUserAsync(Guid id, UpdateUserDTO updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new KeyNotFoundException("User not found.");

            // Mapleme ve güncelleme
            _mapper.Map(updateUserDto, user);
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            await _userRepository.DeleteAsync(id);
        }

        public async Task<string> LoginAsync(LoginUserDTO loginUserDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginUserDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginUserDto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials.");

            // JWT oluşturma
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString()) // Enum'u string'e dönüştürdük
                }),

                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
