using AutoMapper;
using InventoryManagementSystem.Application.DTOs.Category;
using InventoryManagementSystem.Application.Interfaces;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Infrastructure.Repositories.Interfaces;

namespace InventoryManagementSystem.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task AddCategoryAsync(CreateCategoryDTO createCategoryDto)
        {
            var category = _mapper.Map<Category>(createCategoryDto);
            await _categoryRepository.AddAsync(category);
        }

        public async Task UpdateCategoryAsync(Guid id, UpdateCategoryDTO updateCategoryDto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            _mapper.Map(updateCategoryDto, category);
            await _categoryRepository.UpdateAsync(category);
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            await _categoryRepository.DeleteAsync(id);
        }
    }
}
