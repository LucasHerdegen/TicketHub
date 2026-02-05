using AutoMapper;
using TicketHub.API.DTOs.Category;
using TicketHub.API.Models;
using TicketHub.API.Pagination;
using TicketHub.API.Repository;
using TicketHub.API.Services.Interfaces;

namespace TicketHub.API.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _repository;
        private readonly IMapper _mapper;

        public CategoryService(IRepository<Category> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedList<CategoryDto>> GetCategories(PaginationParams paginationParams)
        {
            var pCategories = await _repository.GetPaged(paginationParams);

            var categoriesDto = _mapper.Map<IEnumerable<CategoryDto>>(pCategories.Items);

            return new PagedList<CategoryDto>(
                categoriesDto, pCategories.TotalCount, pCategories.CurrentPage, pCategories.PageSize
            );
        }

        public async Task<CategoryDto?> GetCategory(int id)
        {
            var category = await _repository.GetById(id);

            if (category == null)
                return null;

            var categoryDto = _mapper.Map<CategoryDto>(category);

            return categoryDto;
        }

        public async Task<CategoryDto?> CreateCategory(CategoryPostDto categoryPostDto)
        {
            var exist = await _repository.Any(c => c.Name!.ToUpper() == categoryPostDto.CategoryName!.ToUpper());

            if (exist)
                return null;

            var category = new Category
            {
                Name = categoryPostDto.CategoryName
            };

            await _repository.Create(category);
            await _repository.Save();

            var newCategory = (await _repository.GetById(category.Id))!;
            var categoryDto = _mapper.Map<CategoryDto>(newCategory);

            return categoryDto;
        }

        public async Task<bool> UpdateCategory(CategoryPutDto categoryPutDto)
        {
            var category = await _repository.GetById(categoryPutDto.Id);

            if (category == null)
                return false;

            _mapper.Map(categoryPutDto, category);

            _repository.Update(category);
            await _repository.Save();

            return true;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _repository.GetById(id);

            if (category == null)
                return false;

            _repository.Delete(category);
            await _repository.Save();

            return true;
        }
    }
}