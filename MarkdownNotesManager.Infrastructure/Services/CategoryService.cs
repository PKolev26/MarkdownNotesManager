using MarkdownNotesManager.Core.Interfaces;
using MarkdownNotesManager.Core.Models;

namespace MarkdownNotesManager.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public Task<List<Category>> GetAllCategoriesAsync()
        {
            return _categoryRepository.GetAllCategoriesAsync();
        }

        public Task AddCategoryAsync(Category category)
        {
            return _categoryRepository.AddAsync(category);
        }

        public Task UpdateCategoryAsync(Category category)
        {
            return _categoryRepository.UpdateAsync(category);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category != null)
            {
                await _categoryRepository.DeleteAsync(category);
            }
        }

        public Task<List<Note>> GetNotesByCategoryAsync(int categoryId)
        {
            return _categoryRepository.GetNotesByCategoryAsync(categoryId);
        }
    }
}