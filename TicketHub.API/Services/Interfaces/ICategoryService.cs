using TicketHub.API.DTOs.Category;
using TicketHub.API.Pagination;

namespace TicketHub.API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<PagedList<CategoryDto>> GetCategories(PaginationParams paginationParams);
        Task<CategoryDto?> GetCategory(int id);
        Task<CategoryDto?> CreateCategory(CategoryPostDto categoryPostDto);
        Task<bool> UpdateCategory(CategoryPutDto categoryPutDto);
        Task<bool> DeleteCategory(int id);
    }
}