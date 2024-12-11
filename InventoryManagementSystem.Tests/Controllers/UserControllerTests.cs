using InventoryManagementSystem.API.Controllers;
using InventoryManagementSystem.Application.DTOs.User;
using InventoryManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace InventoryManagementSystem.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UserController(_mockUserService.Object);
        }

        [Fact]
        public async Task Register_WithAdminRole_ReturnsOk()
        {
            // Arrange
            var registerDto = new RegisterUserDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Password = "password123",
                Role = "Manager"
            };

            var claims = new List<Claim> { new Claim("role", "Admin") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockUserService.Setup(x => x.RegisterUserAsync(registerDto, "Admin")).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User registered successfully.", okResult.Value);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginDto = new LoginUserDTO
            {
                Email = "john.doe@example.com",
                Password = "password123"
            };

            _mockUserService
                .Setup(x => x.LoginAsync(It.IsAny<LoginUserDTO>()))
                .ReturnsAsync("fake-jwt-token");

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            // Anonim türden Token'i kontrol et
            var response = okResult.Value;
            var tokenProperty = response.GetType().GetProperty("Token");
            Assert.NotNull(tokenProperty);

            var tokenValue = tokenProperty.GetValue(response)?.ToString();
            Assert.Equal("fake-jwt-token", tokenValue);
        }

        [Fact]
        public async Task GetUserById_WithAdminRole_ReturnsUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userDetails = new UserDetailsDTO
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Role = "Viewer"
            };

            var claims = new List<Claim> { new Claim("role", "Admin") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockUserService
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(userDetails);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            var response = Assert.IsType<UserDetailsDTO>(okResult.Value);
            Assert.Equal(userDetails.FirstName, response.FirstName);
            Assert.Equal(userDetails.LastName, response.LastName);
            Assert.Equal(userDetails.Role, response.Role);
        }


    }
}
