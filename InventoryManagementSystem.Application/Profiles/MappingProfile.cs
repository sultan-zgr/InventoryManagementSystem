using AutoMapper;
using InventoryManagementSystem.Application.DTOs.User;
using InventoryManagementSystem.Application.DTOs.Product;
using InventoryManagementSystem.Application.DTOs.Category;
using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User Mappings
            CreateMap<User, RegisterUserDTO>().ReverseMap();
            CreateMap<User, UserDetailsDTO>().ReverseMap();
            CreateMap<User, UpdateUserDTO>().ReverseMap();

            // Product Mappings
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Product, CreateProductDTO>().ReverseMap();
            CreateMap<Product, UpdateProductDTO>().ReverseMap();

            // Category Mappings
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Category, CreateCategoryDTO>().ReverseMap();
            CreateMap<Category, UpdateCategoryDTO>().ReverseMap();
        }
    }
}
