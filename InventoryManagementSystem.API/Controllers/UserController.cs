using InventoryManagementSystem.Application.DTOs.User;
using InventoryManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventoryManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO registerUserDto)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            // Kullanıcıyı kaydet
            await _userService.RegisterUserAsync(registerUserDto, role);
            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO loginUserDto)
        {
            var token = await _userService.LoginAsync(loginUserDto);

            if (string.IsNullOrEmpty(token))
                return Unauthorized("Invalid credentials."); // Başarısız girişte Unauthorized dön

            return Ok(new { Token = token });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound("User not found."); // Kullanıcı bulunamazsa 404

            return Ok(user);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var users = await _userService.GetAllUsersAsync(page, pageSize); // Sayfalama eklenebilir
            return Ok(users);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDTO updateUserDto)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound("User not found."); // Kullanıcı bulunamazsa 404

            await _userService.UpdateUserAsync(id, updateUserDto);
            return Ok("User updated successfully.");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound("User not found."); // Kullanıcı bulunamazsa 404

            await _userService.DeleteUserAsync(id);
            return Ok("User deleted successfully.");
        }
    }
}
