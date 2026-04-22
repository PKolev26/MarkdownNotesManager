using MarkdownNotesManager.Core.Models;

namespace MarkdownNotesManager.Core.Interfaces
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);
        Task<List<Note>> GetNotesByCategoryAsync(int categoryId);
    }
}