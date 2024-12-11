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
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO registerUserDto)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            try
            {
                await _userService.RegisterUserAsync(registerUserDto, role);
                _logger.LogInformation("User registered successfully: {Email}", registerUserDto.Email);
                return Ok("User registered successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user: {Email}", registerUserDto.Email);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
        {
            try
            {
                await _userService.ConfirmEmailAsync(token);
                _logger.LogInformation("Email verified successfully for token: {Token}", token);
                return Ok("Email verified successfully.");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid or expired token: {Token}", token);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while confirming email for token: {Token}", token);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO loginUserDto)
        {
            try
            {
                var token = await _userService.LoginAsync(loginUserDto);

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Login failed for user: {Email}", loginUserDto.Email);
                    return Unauthorized("Invalid credentials.");
                }

                _logger.LogInformation("User logged in successfully: {Email}", loginUserDto.Email);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while logging in user: {Email}", loginUserDto.Email);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", id);
                    return NotFound("User not found.");
                }

                _logger.LogInformation("User retrieved successfully: {UserId}", id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving user with ID {UserId}.", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var users = await _userService.GetAllUsersAsync(page, pageSize);
                _logger.LogInformation("All users retrieved successfully.");
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all users.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDTO updateUserDto)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", id);
                    return NotFound("User not found.");
                }

                await _userService.UpdateUserAsync(id, updateUserDto);
                _logger.LogInformation("User updated successfully: {UserId}", id);
                return Ok("User updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user with ID {UserId}.", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", id);
                    return NotFound("User not found.");
                }

                await _userService.DeleteUserAsync(id);
                _logger.LogInformation("User deleted successfully: {UserId}", id);
                return Ok("User deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user with ID {UserId}.", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
