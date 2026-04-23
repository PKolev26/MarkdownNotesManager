using MarkdownNotesManager.Core.Models;

namespace MarkdownNotesManager.Core.Interfaces
{
    public interface INoteService
    {
        Task<List<Note>> GetAllNotesAsync();
        Task<List<Note>> GetNotesByCategoryAsync(int categoryId);
        Task<Note?> GetNoteByIdAsync(int id);
        Task AddNoteAsync(Note note);
        Task UpdateNoteAsync(Note note);
        Task DeleteNoteAsync(int id);
    }
}