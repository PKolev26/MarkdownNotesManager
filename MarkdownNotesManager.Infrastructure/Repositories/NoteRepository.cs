using MarkdownNotesManager.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarkdownNotesManager.Core.Models;
using MarkdownNotesManager.Infrastructure.Data;

namespace MarkdownNotesManager.Infrastructure.Repositories
{
    public class NoteRepository : INoteRepository
    {
        public async Task<List<Note>> GetAllNotesAsync()
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

        public async Task<List<Note>> GetNotesByCategoryIdAsync(int categoryId)
        {
            using var db = new AppDbContext();
            return await db.Notes.Where(n => n.CategoryId == categoryId).Include(n => n.Category).ToListAsync();
        }
    }
}
