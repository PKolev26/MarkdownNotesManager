using MarkdownNotesManager.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return _noteRepository.GetAllNotesAsync();
        }

        public Task<Note?> GetNoteByIdAsync(int id)
        {
            return _noteRepository.GetByIdAsync(id);
        }

        public Task AddNoteAsync(Note note)
        {
            note.CreatedAt = DateTime.Now;
            note.UpdatedAt = DateTime.Now;
            return _noteRepository.AddAsync(note);
        }

        public Task UpdateNoteAsync(Note note)
        {
            note.UpdatedAt = DateTime.Now;
            return _noteRepository.UpdateAsync(note);
        }

        public Task DeleteNoteAsync(int id)
        {
            return _noteRepository.DeleteAsync(id);
        }

        public Task<object> GetNotesByCategoryAsync(int id)
        {
            return _noteRepository.GetNotesByCategoryIdAsync(id).ContinueWith(t => (object)t.Result);
        }
    }
}
