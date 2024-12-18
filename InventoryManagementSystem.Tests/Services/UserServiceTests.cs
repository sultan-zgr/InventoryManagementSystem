﻿using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using InventoryManagementSystem.Application.DTOs.User;
using InventoryManagementSystem.Application.Services;
using InventoryManagementSystem.Application.Validators;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enums;
using InventoryManagementSystem.Infrastructure.Repositories.Interfaces;
using InventoryManagementSystem.Infrastructure.Services;
using Moq;
using Xunit;

namespace InventoryManagementSystem.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<ITokenGenerator> _tokenGeneratorMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _emailSenderMock = new Mock<IEmailSender>();
            _tokenGeneratorMock = new Mock<ITokenGenerator>();

            _userService = new UserService(
                _userRepositoryMock.Object,
                _mapperMock.Object,
                Mock.Of<Microsoft.Extensions.Configuration.IConfiguration>(),
                _emailSenderMock.Object,
                _tokenGeneratorMock.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_Should_Add_User_When_Valid()
        {
            // Arrange
            var registerUserDto = new RegisterUserDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "Password123",
                Role = "Viewer"
            };

            var userEntity = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PasswordHash = "hashedPassword",
                Role = UserRole.Viewer
            };

            _mapperMock.Setup(m => m.Map<User>(registerUserDto)).Returns(userEntity);
            _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Act
            await _userService.RegisterUserAsync(registerUserDto, "Admin");

            // Assert
            _userRepositoryMock.Verify(r => r.AddAsync(It.Is<User>(u =>
                u.Email == registerUserDto.Email &&
                u.Role == UserRole.Viewer
            )), Times.Once);

            _mapperMock.Verify(m => m.Map<User>(registerUserDto), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_Should_Throw_When_Non_Admin_Assigns_Non_Viewer_Role()
        {
            // Arrange
            var registerUserDto = new RegisterUserDTO
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Password = "Password123",
                Role = "Admin" 
            };

            // Act
            Func<Task> act = async () => await _userService.RegisterUserAsync(registerUserDto, "Manager");

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Only Admins can assign roles other than Viewer.");
        }
        [Fact]
        public void Should_Return_Error_When_Email_Is_Invalid()
        {
            var validator = new LoginUserDTOValidator();
            var result = validator.Validate(new LoginUserDTO { Email = "invalidemail", Password = "Password1!" });

            result.Errors.Should().ContainSingle()
                .Which.ErrorMessage.Should().Be("Invalid email format.");
        }

        [Fact]
        public async Task UpdateUserRoleAsync_Should_Update_Role_When_Valid()
        {
            var user = new User { Email = "test@example.com", Role = UserRole.Viewer };
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);

            var dto = new UpdateUserRoleDTO { Email = user.Email, NewRole = "Admin" };

            await _userService.UpdateUserRoleAsync(dto, UserRole.Admin.ToString());

            _userRepositoryMock.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Role == UserRole.Admin)), Times.Once);
        }


    }
}
