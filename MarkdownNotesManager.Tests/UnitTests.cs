using Microsoft.VisualStudio.TestTools.UnitTesting;
using MarkdownNotesManager.App.Commands;
using MarkdownNotesManager.App.ViewModels;
using MarkdownNotesManager.Core.Interfaces;
using MarkdownNotesManager.Core.Models;
using MarkdownNotesManager.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MarkdownNotesManager.Tests
{
    [TestClass]
    public class RelayCommandTests
    {
        [TestMethod]
        public void Execute_CallsProvidedAction()
        {
            object? received = null;
            var command = new RelayCommand(p => received = p);

            command.Execute("hello");

            Assert.AreEqual("hello", received);
        }

        [TestMethod]
        public void CanExecute_UsesPredicate_AndRaiseCanExecuteChanged_FiresEvent()
        {
            var eventRaised = false;
            var command = new RelayCommand(_ => { }, p => (string?)p == "ok");
            command.CanExecuteChanged += (_, __) => eventRaised = true;

            Assert.IsTrue(command.CanExecute("ok"));
            Assert.IsFalse(command.CanExecute("no"));

            command.RaiseCanExecuteChanged();

            Assert.IsTrue(eventRaised);
        }
    }

    [TestClass]
    public class NoteModelTests
    {
        [TestMethod]
        public void SettingProperty_RaisesPropertyChanged()
        {
            var note = new Note();
            string? changedProperty = null;
            note.PropertyChanged += (_, e) => changedProperty = e.PropertyName;

            note.Title = "Updated title";

            Assert.AreEqual(nameof(Note.Title), changedProperty);
        }
    }

    [TestClass]
    public class MarkdownServiceTests
    {
        [TestMethod]
        public void ToHtml_ConvertsMarkdown_AndWrapsInHtmlDocument()
        {
            var service = new MarkdownService();

            var html = service.ToHtml("# Title\n\n**bold**");

            StringAssert.Contains(html, "<!DOCTYPE html>");
            StringAssert.Contains(html, "<html>");
            StringAssert.Contains(html, "<h1>Title</h1>");
            StringAssert.Contains(html.ToLowerInvariant(), "<strong>bold</strong>");
        }

        [TestMethod]
        public void ToHtml_WhenMarkdownIsNull_ReturnsValidHtml()
        {
            var service = new MarkdownService();

            var html = service.ToHtml(null!);

            StringAssert.Contains(html, "<!DOCTYPE html>");
            StringAssert.Contains(html, "<body>");
        }
    }

    [TestClass]
    public class NoteServiceTests
    {
        [TestMethod]
        public async Task AddNoteAsync_SetsCreatedAtAndUpdatedAt_AndCallsRepository()
        {
            var repo = new FakeNoteRepository();
            var service = new NoteService(repo);
            var note = new Note { Title = "Test", Content = "Body" };
            var before = DateTime.Now;

            await service.AddNoteAsync(note);

            var after = DateTime.Now;
            Assert.AreSame(note, repo.AddedNote);
            Assert.IsTrue(note.CreatedAt >= before && note.CreatedAt <= after);
            Assert.IsTrue(note.UpdatedAt >= before && note.UpdatedAt <= after);
        }

        [TestMethod]
        public async Task UpdateNoteAsync_RefreshesUpdatedAt_AndCallsRepository()
        {
            var repo = new FakeNoteRepository();
            var service = new NoteService(repo);
            var oldUpdatedAt = DateTime.Now.AddDays(-1);
            var note = new Note { Id = 5, Title = "Test", UpdatedAt = oldUpdatedAt };
            var before = DateTime.Now;

            await service.UpdateNoteAsync(note);

            var after = DateTime.Now;
            Assert.AreSame(note, repo.UpdatedNote);
            Assert.IsTrue(note.UpdatedAt > oldUpdatedAt);
            Assert.IsTrue(note.UpdatedAt >= before && note.UpdatedAt <= after);
        }

        [TestMethod]
        public async Task DeleteNoteAsync_PassesIdToRepository()
        {
            var repo = new FakeNoteRepository();
            var service = new NoteService(repo);

            await service.DeleteNoteAsync(42);

            Assert.AreEqual(42, repo.DeletedId);
        }
    }

    [TestClass]
    public class CategoryServiceTests
    {
        [TestMethod]
        public async Task DeleteCategoryAsync_DeletesCategory_WhenItExists()
        {
            var repo = new FakeCategoryRepository();
            var category = new Category { Id = 7, Name = "Work" };
            repo.CategoryById = category;
            var service = new CategoryService(repo);

            await service.DeleteCategoryAsync(7);

            Assert.AreSame(category, repo.DeletedCategory);
        }

        [TestMethod]
        public async Task DeleteCategoryAsync_DoesNothing_WhenCategoryIsMissing()
        {
            var repo = new FakeCategoryRepository();
            var service = new CategoryService(repo);

            await service.DeleteCategoryAsync(7);

            Assert.IsNull(repo.DeletedCategory);
        }

        [TestMethod]
        public async Task GetNotesByCategoryAsync_ReturnsRepositoryResult()
        {
            var repo = new FakeCategoryRepository();
            repo.NotesByCategory = new List<Note>
            {
                new Note { Id = 1, Title = "A", CategoryId = 3 },
                new Note { Id = 2, Title = "B", CategoryId = 3 }
            };
            var service = new CategoryService(repo);

            var result = await service.GetNotesByCategoryAsync(3);

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(n => n.CategoryId == 3));
        }
    }

    [TestClass]
    public class MainViewModelTests
    {
        [TestMethod]
        public async Task LoadDataAsync_LoadsCategories_AndSortsNotesAscendingByCategoryThenTitle()
        {
            var work = new Category { Id = 1, Name = "Work" };
            var personal = new Category { Id = 2, Name = "Personal" };

            var noteService = new FakeNoteService
            {
                AllNotes = new List<Note>
                {
                    new Note { Id = 1, Title = "Zeta", Content = "# Zeta", Category = work, CategoryId = work.Id },
                    new Note { Id = 2, Title = "Alpha", Content = "# Alpha", Category = personal, CategoryId = personal.Id },
                    new Note { Id = 3, Title = "Beta", Content = "# Beta", Category = personal, CategoryId = personal.Id }
                }
            };

            var categoryService = new FakeCategoryService
            {
                Categories = new List<Category> { work, personal }
            };

            var vm = new MainViewModel(noteService, categoryService, new MarkdownService());
            await vm.LoadDataAsync();

            CollectionAssert.AreEqual(new[] { "Alpha", "Beta", "Zeta" }, vm.Notes.Select(n => n.Title).ToList());
            Assert.AreEqual(2, vm.Categories.Count);
            Assert.IsNotNull(vm.SelectedNote);
            Assert.AreEqual("Alpha", vm.SelectedNote!.Title);
        }

        [TestMethod]
        public async Task SortByCategoryCommand_TogglesToDescendingOrder()
        {
            var work = new Category { Id = 1, Name = "Work" };
            var personal = new Category { Id = 2, Name = "Personal" };

            var noteService = new FakeNoteService
            {
                AllNotes = new List<Note>
                {
                    new Note { Id = 1, Title = "Zeta", Content = "# Zeta", Category = work, CategoryId = work.Id },
                    new Note { Id = 2, Title = "Alpha", Content = "# Alpha", Category = personal, CategoryId = personal.Id },
                    new Note { Id = 3, Title = "Beta", Content = "# Beta", Category = personal, CategoryId = personal.Id }
                }
            };

            var vm = new MainViewModel(noteService, new FakeCategoryService(), new MarkdownService());
            await vm.LoadDataAsync();

            vm.SortByCategoryCommand.Execute(null);
            await Task.Delay(50);

            CollectionAssert.AreEqual(new[] { "Zeta", "Alpha", "Beta" }, vm.Notes.Select(n => n.Title).ToList());
        }

        [TestMethod]
        public void UpdatePreview_WhenNoNoteIsSelected_ShowsPlaceholderHtml()
        {
            var vm = new MainViewModel(new FakeNoteService(), new FakeCategoryService(), new MarkdownService())
            {
                SelectedNote = null
            };

            vm.UpdatePreview();

            StringAssert.Contains(vm.PreviewHtml, "No note selected.");
        }

        [TestMethod]
        public async Task NewNoteCommand_CreatesDefaultNote_UsingSelectedCategory()
        {
            var category = new Category { Id = 11, Name = "Ideas" };
            var vm = new MainViewModel(new FakeNoteService(), new FakeCategoryService(), new MarkdownService())
            {
                SelectedCategory = category
            };

            await Task.Delay(50);
            vm.NewNoteCommand.Execute(null);

            Assert.IsNotNull(vm.SelectedNote);
            Assert.AreEqual("New Note ✏️", vm.SelectedNote!.Title);
            Assert.AreEqual(category.Id, vm.SelectedNote.CategoryId);
            Assert.AreSame(category, vm.SelectedNote.Category);
            Assert.IsFalse(vm.IsPreviewMode);
        }

        [TestMethod]
        public void ChangingSelectedNoteContent_RefreshesPreviewHtml()
        {
            var vm = new MainViewModel(new FakeNoteService(), new FakeCategoryService(), new MarkdownService());
            var note = new Note { Title = "T", Content = "# Old" };
            vm.SelectedNote = note;

            note.Content = "# New title";

            StringAssert.Contains(vm.PreviewHtml, "<h1>New title</h1>");
        }
    }

    internal sealed class FakeNoteRepository : INoteRepository
    {
        public List<Note> AllNotes { get; set; } = new();
        public Note? NoteById { get; set; }
        public Note? AddedNote { get; private set; }
        public Note? UpdatedNote { get; private set; }
        public int DeletedId { get; private set; } = -1;

        public Task<List<Note>> GetAllAsync() => Task.FromResult(AllNotes.ToList());

        public Task<List<Note>> GetNotesByCategoryAsync(int categoryId) =>
            Task.FromResult(AllNotes.Where(n => n.CategoryId == categoryId).ToList());

        public Task<Note?> GetByIdAsync(int id) =>
            Task.FromResult(NoteById ?? AllNotes.FirstOrDefault(n => n.Id == id));

        public Task AddAsync(Note note)
        {
            AddedNote = note;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Note note)
        {
            UpdatedNote = note;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            DeletedId = id;
            return Task.CompletedTask;
        }
    }

    internal sealed class FakeCategoryRepository : ICategoryRepository
    {
        public Category? CategoryById { get; set; }
        public List<Category> Categories { get; set; } = new();
        public List<Note> NotesByCategory { get; set; } = new();
        public Category? AddedCategory { get; private set; }
        public Category? UpdatedCategory { get; private set; }
        public Category? DeletedCategory { get; private set; }

        public Task<Category?> GetByIdAsync(int id) =>
            Task.FromResult(CategoryById ?? Categories.FirstOrDefault(c => c.Id == id));

        public Task<List<Category>> GetAllCategoriesAsync() => Task.FromResult(Categories.ToList());

        public Task AddAsync(Category category)
        {
            AddedCategory = category;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Category category)
        {
            UpdatedCategory = category;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Category category)
        {
            DeletedCategory = category;
            return Task.CompletedTask;
        }

        public Task<List<Note>> GetNotesByCategoryAsync(int categoryId) =>
            Task.FromResult(NotesByCategory.Where(n => n.CategoryId == categoryId).ToList());
    }

    internal sealed class FakeNoteService : INoteService
    {
        public List<Note> AllNotes { get; set; } = new();
        public int DeletedId { get; private set; } = -1;
        public Note? AddedNote { get; private set; }
        public Note? UpdatedNote { get; private set; }

        public Task<List<Note>> GetAllNotesAsync() => Task.FromResult(AllNotes.ToList());

        public Task<List<Note>> GetNotesByCategoryAsync(int categoryId) =>
            Task.FromResult(AllNotes.Where(n => n.CategoryId == categoryId).ToList());

        public Task<Note?> GetNoteByIdAsync(int id) =>
            Task.FromResult(AllNotes.FirstOrDefault(n => n.Id == id));

        public Task AddNoteAsync(Note note)
        {
            AddedNote = note;
            if (note.Id == 0)
                note.Id = AllNotes.Count == 0 ? 1 : AllNotes.Max(n => n.Id) + 1;
            AllNotes.Add(note);
            return Task.CompletedTask;
        }

        public Task UpdateNoteAsync(Note note)
        {
            UpdatedNote = note;
            return Task.CompletedTask;
        }

        public Task DeleteNoteAsync(int id)
        {
            DeletedId = id;
            AllNotes.RemoveAll(n => n.Id == id);
            return Task.CompletedTask;
        }
    }

    internal sealed class FakeCategoryService : ICategoryService
    {
        public List<Category> Categories { get; set; } = new();
        public List<Note> NotesByCategory { get; set; } = new();
        public int DeletedId { get; private set; } = -1;
        public Category? AddedCategory { get; private set; }
        public Category? UpdatedCategory { get; private set; }

        public Task<List<Category>> GetAllCategoriesAsync() => Task.FromResult(Categories.ToList());

        public Task AddCategoryAsync(Category category)
        {
            AddedCategory = category;
            Categories.Add(category);
            return Task.CompletedTask;
        }

        public Task UpdateCategoryAsync(Category category)
        {
            UpdatedCategory = category;
            return Task.CompletedTask;
        }

        public Task DeleteCategoryAsync(int id)
        {
            DeletedId = id;
            Categories.RemoveAll(c => c.Id == id);
            return Task.CompletedTask;
        }

        public Task<List<Note>> GetNotesByCategoryAsync(int categoryId) =>
            Task.FromResult(NotesByCategory.Where(n => n.CategoryId == categoryId).ToList());
    }
}
