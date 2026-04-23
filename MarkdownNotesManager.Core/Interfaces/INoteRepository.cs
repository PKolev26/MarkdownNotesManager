using MarkdownNotesManager.Core.Models;

namespace MarkdownNotesManager.Core.Interfaces
{
    public interface INoteRepository
    {
        Task<List<Note>> GetAllAsync();
        Task<List<Note>> GetNotesByCategoryAsync(int categoryId);
        Task<Note?> GetByIdAsync(int id);
        Task AddAsync(Note note);
        Task UpdateAsync(Note note);
        Task DeleteAsync(int id);
    }
}