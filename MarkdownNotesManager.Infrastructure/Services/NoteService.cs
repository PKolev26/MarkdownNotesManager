using MarkdownNotesManager.Core.Interfaces;
using MarkdownNotesManager.Core.Models;

namespace MarkdownNotesManager.Infrastructure.Services
{
    public class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepository;

        public NoteService(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public Task<List<Note>> GetAllNotesAsync()
        {
            return _noteRepository.GetAllAsync();
        }

        public Task<List<Note>> GetNotesByCategoryAsync(int categoryId)
        {
            return _noteRepository.GetNotesByCategoryAsync(categoryId);
        }

        public Task<Note?> GetNoteByIdAsync(int id)
        {
            return _noteRepository.GetByIdAsync(id);
        }

        public async Task AddNoteAsync(Note note)
        {
            note.CreatedAt = DateTime.Now;
            note.UpdatedAt = DateTime.Now;
            await _noteRepository.AddAsync(note);
        }

        public async Task UpdateNoteAsync(Note note)
        {
            note.UpdatedAt = DateTime.Now;
            await _noteRepository.UpdateAsync(note);
        }

        public Task DeleteNoteAsync(int id)
        {
            return _noteRepository.DeleteAsync(id);
        }
    }
}