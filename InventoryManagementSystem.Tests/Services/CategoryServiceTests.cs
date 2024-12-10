using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using InventoryManagementSystem.Application.DTOs.Category;
using InventoryManagementSystem.Application.Services;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Infrastructure.Repositories.Interfaces;
using Moq;
using Xunit;

namespace InventoryManagementSystem.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _mapperMock = new Mock<IMapper>();
            _categoryService = new CategoryService(_categoryRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_Should_Return_All_Categories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Electronics", Description = "All electronics" },
                new Category { Id = Guid.NewGuid(), Name = "Books", Description = "All books" }
            };

            var categoryDtos = new List<CategoryDTO>
            {
                new CategoryDTO { Id = categories[0].Id, Name = "Electronics", Description = "All electronics" },
                new CategoryDTO { Id = categories[1].Id, Name = "Books", Description = "All books" }
            };

            _categoryRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(categories);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<CategoryDTO>>(categories)).Returns(categoryDtos);

            // Act
            var result = await _categoryService.GetAllCategoriesAsync();

            // Assert
            result.Should().BeEquivalentTo(categoryDtos);
            _categoryRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<IEnumerable<CategoryDTO>>(categories), Times.Once);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_Should_Return_Category_When_Exists()
        {
            // Arrange
            var category = new Category { Id = Guid.NewGuid(), Name = "Electronics", Description = "All electronics" };
            var categoryDto = new CategoryDTO { Id = category.Id, Name = "Electronics", Description = "All electronics" };

            _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(category.Id)).ReturnsAsync(category);
            _mapperMock.Setup(mapper => mapper.Map<CategoryDTO>(category)).Returns(categoryDto);

            // Act
            var result = await _categoryService.GetCategoryByIdAsync(category.Id);

            // Assert
            result.Should().BeEquivalentTo(categoryDto);
            _categoryRepositoryMock.Verify(repo => repo.GetByIdAsync(category.Id), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<CategoryDTO>(category), Times.Once);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_Should_Throw_KeyNotFoundException_When_Not_Found()
        {
            // Arrange
            _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Category)null);

            // Act
            Func<Task> act = async () => await _categoryService.GetCategoryByIdAsync(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Category not found.");
        }

        [Fact]
        public async Task AddCategoryAsync_Should_Add_Category()
        {
            // Arrange
            var createCategoryDto = new CreateCategoryDTO { Name = "Furniture", Description = "All furniture" };
            var category = new Category { Id = Guid.NewGuid(), Name = "Furniture", Description = "All furniture" };

            _mapperMock.Setup(mapper => mapper.Map<Category>(createCategoryDto)).Returns(category);
            _categoryRepositoryMock.Setup(repo => repo.AddAsync(category)).Returns(Task.CompletedTask);

            // Act
            await _categoryService.AddCategoryAsync(createCategoryDto);

            // Assert
            _mapperMock.Verify(mapper => mapper.Map<Category>(createCategoryDto), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.AddAsync(category), Times.Once);
        }

        [Fact]
        public async Task UpdateCategoryAsync_Should_Update_Category()
        {
            // Arrange
            var id = Guid.NewGuid();
            var existingCategory = new Category { Id = id, Name = "Old Name", Description = "Old Description" };
            var updateCategoryDto = new UpdateCategoryDTO { Name = "New Name", Description = "New Description" };

            _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(existingCategory);
            _categoryRepositoryMock.Setup(repo => repo.UpdateAsync(existingCategory)).Returns(Task.CompletedTask);

            // Act
            await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);

            // Assert
            _mapperMock.Verify(mapper => mapper.Map(updateCategoryDto, existingCategory), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.UpdateAsync(existingCategory), Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryAsync_Should_Delete_Category()
        {
            // Arrange
            var id = Guid.NewGuid();
            var existingCategory = new Category { Id = id, Name = "To Delete", Description = "To be deleted" };

            _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(existingCategory);
            _categoryRepositoryMock.Setup(repo => repo.DeleteAsync(id)).Returns(Task.CompletedTask);

            // Act
            await _categoryService.DeleteCategoryAsync(id);

            // Assert
            _categoryRepositoryMock.Verify(repo => repo.GetByIdAsync(id), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.DeleteAsync(id), Times.Once);
        }
    }
}
