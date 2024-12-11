using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using InventoryManagementSystem.Application.DTOs.User;
using InventoryManagementSystem.Application.Interfaces;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enums;
using InventoryManagementSystem.Infrastructure.Repositories.Interfaces;
using InventoryManagementSystem.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace InventoryManagementSystem.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender; // E-posta gönderici
        private readonly ITokenGenerator _tokenGenerator; // Token oluşturucu

        public UserService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration, IEmailSender emailSender, ITokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
            _emailSender = emailSender;
            _tokenGenerator = tokenGenerator;
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

        public async Task<IEnumerable<UserDetailsDTO>> GetAllUsersAsync(int page = 1, int pageSize = 10)
        {
            // Sayfalama mantığı
            var users = await _userRepository.GetAllPagedAsync(page, pageSize);
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
            await _userRepository.DeleteAsync(id); // Direkt id üzerinden silme işlemi
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
        public async Task<string> GenerateEmailConfirmationTokenAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            // Token oluştur ve kullanıcıya ata
            var token = Guid.NewGuid().ToString();
            user.EmailConfirmationToken = token;

            await _userRepository.UpdateAsync(user);
            return token;
        }
        public async Task RegisterUserAsync(RegisterUserDTO registerUserDto)
        {
            var userEntity = _mapper.Map<User>(registerUserDto);
            userEntity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password);
            userEntity.EmailConfirmationToken = _tokenGenerator.GenerateToken(); // Token oluştur
            userEntity.TokenCreatedAt = DateTime.UtcNow;

            await _userRepository.AddAsync(userEntity);

            var verificationLink = $"https://yourapp.com/api/user/confirm-email?token={userEntity.EmailConfirmationToken}";
            await _emailSender.SendEmailAsync(
                userEntity.Email,
                "Confirm Your Email",
                $"Please click <a href=\"{verificationLink}\">here</a> to verify your email. This link is valid for 24 hours."
            );
        }

        public async Task ConfirmEmailAsync(string token)
        {
            var user = await _userRepository.GetByEmailConfirmationTokenAsync(token);
            if (user == null || user.IsEmailConfirmed)
                throw new InvalidOperationException("Invalid or expired token.");

            // Token geçerlilik süresi kontrolü
            if (DateTime.UtcNow > user.TokenCreatedAt?.AddHours(24))
                throw new InvalidOperationException("Token has expired.");

            user.IsEmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.TokenCreatedAt = null;

            await _userRepository.UpdateAsync(user);
        }
        public async Task UpdateUserRoleAsync(UpdateUserRoleDTO updateUserRoleDto, string adminRole)
        {
            if (adminRole != UserRole.Admin.ToString())
                throw new UnauthorizedAccessException("Only Admins can update roles.");

            var user = await _userRepository.GetByEmailAsync(updateUserRoleDto.Email);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            user.UpdateRole(updateUserRoleDto.NewRole); // UpdateRole içinde Enum dönüşümü yapılır
            await _userRepository.UpdateAsync(user);
        }
    }
}
