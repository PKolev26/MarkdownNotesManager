using MarkdownNotesManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownNotesManager.Core.Interfaces
{
    public interface INoteRepository
    {
        Task<List<Note>> GetAllNotesAsync();
        Task<Note?> GetByIdAsync(int id);
        Task AddAsync(Note note);
        Task UpdateAsync(Note note);    
        Task DeleteAsync(int id);

        Task<List<Note>> GetNotesByCategoryIdAsync(int categoryId);
    }
}
