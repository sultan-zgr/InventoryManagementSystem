using InventoryManagementSystem.Application.DTOs.Category;

namespace InventoryManagementSystem.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryDTO> GetCategoryByIdAsync(Guid id);
        Task AddCategoryAsync(CreateCategoryDTO createCategoryDto);
        Task UpdateCategoryAsync(Guid id, UpdateCategoryDTO updateCategoryDto);
        Task DeleteCategoryAsync(Guid id);
    }
}
