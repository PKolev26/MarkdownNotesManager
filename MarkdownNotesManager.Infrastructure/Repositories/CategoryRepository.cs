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
    public class CategoryRepository : ICategoryRepository
    {
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            using var db = new AppDbContext();
            return await db.Categories.ToListAsync();
        }

        public async Task AddAsync(Category category)
        {
            using var db = new AppDbContext();
            db.Categories.Add(category);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Category category)
        {
            using var db = new AppDbContext();
            db.Categories.Remove(category);
            await db.SaveChangesAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            using var db = new AppDbContext();
            return await db.Categories.FindAsync(id);
        }

        public async Task<List<Note>> GetNotesByCategoryAsync(int categoryId)
        {
            using var db = new AppDbContext();
            return await db.Notes.Where(n => n.CategoryId == categoryId).Include(n => n.Category).ToListAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            using var db = new AppDbContext();
            db.Categories.Update(category);
            await db.SaveChangesAsync();
        }
    }
}
