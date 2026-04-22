using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkdownNotesManager.Core.Models;

namespace MarkdownNotesManager.Core.Interfaces
{
    public interface INoteService
    {
        Task<List<Note>> GetAllNotesAsync();
        Task<Note?> GetNoteByIdAsync(int id);
        Task AddNoteAsync(Note note);
        Task UpdateNoteAsync(Note note);
        Task DeleteNoteAsync(int id);
    }
}
