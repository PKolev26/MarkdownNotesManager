using MarkdownNotesManager.Core.Interfaces;
using MarkdownNotesManager.Core.Models;
using MarkdownNotesManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MarkdownNotesManager.Infrastructure.Repositories
{
    public class NoteRepository : INoteRepository
    {
        public async Task<List<Note>> GetAllAsync()
        {
            using var db = new AppDbContext();
            return await db.Notes.Include(n => n.Category).ToListAsync();
        }

        public async Task<Note?> GetByIdAsync(int id)
        {
            using var db = new AppDbContext();
            return await db.Notes.Include(n => n.Category).FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task AddAsync(Note note)
        {
            using var db = new AppDbContext();

            if (note.Category != null)
            {
                db.Entry(note.Category).State = EntityState.Unchanged;
            }

            db.Notes.Add(note);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Note note)
        {
            using var db = new AppDbContext();
            db.Notes.Update(note);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var db = new AppDbContext();
            var note = await db.Notes.FindAsync(id);
            if (note != null)
            {
                db.Notes.Remove(note);
                await db.SaveChangesAsync();
            }
        }
        public async Task<List<Note>> GetNotesByCategoryAsync(int categoryId)
        {
            using var db = new AppDbContext();
            return await db.Notes.Where(n => n.CategoryId == categoryId).Include(n => n.Category).ToListAsync();
        }
    }
}
