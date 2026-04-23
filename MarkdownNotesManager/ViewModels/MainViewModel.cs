using MarkdownNotesManager.App.Commands;
using MarkdownNotesManager.Core.Interfaces;
using MarkdownNotesManager.Core.Models;
using MarkdownNotesManager.Infrastructure.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MarkdownNotesManager.App.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly INoteService _noteService;
        private readonly ICategoryService _categoryService;
        private readonly MarkdownService _markdownService;

        private Note? _selectedNote;
        private string _previewHtml = "<html><body></body></html>";
        private bool _isPreviewMode;
        private Category? _selectedCategory;
        private bool _sortAscending = true;

        public ObservableCollection<Note> Notes { get; set; } = new();
        public ObservableCollection<Category> Categories { get; set; } = new();

        public ICommand LoadCommand { get; }
        public ICommand NewNoteCommand { get; }
        public ICommand SaveNoteCommand { get; }
        public ICommand DeleteNoteCommand { get; }
        public ICommand AddCategoryCommand { get; }
        public ICommand RemoveCategoryCommand { get; }
        public ICommand ShowEditCommand { get; }
        public ICommand ShowPreviewCommand { get; }
        public ICommand SortByCategoryCommand { get; }

        public Note? SelectedNote
        {
            get => _selectedNote;
            set
            {
                if (_selectedNote != null)
                    _selectedNote.PropertyChanged -= SelectedNote_PropertyChanged;

                _selectedNote = value;

                if (_selectedNote != null)
                    _selectedNote.PropertyChanged += SelectedNote_PropertyChanged;

                OnPropertyChanged();
                UpdatePreview();
            }
        }

        public string PreviewHtml
        {
            get => _previewHtml;
            set
            {
                _previewHtml = value;
                OnPropertyChanged();
            }
        }

        public bool IsPreviewMode
        {
            get => _isPreviewMode;
            set
            {
                _isPreviewMode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEditMode));
            }
        }

        public bool IsEditMode => !IsPreviewMode;

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
                _ = FilterNotesAsync();
            }
        }

        public MainViewModel(
            INoteService noteService,
            ICategoryService categoryService,
            MarkdownService markdownService)
        {
            _noteService = noteService;
            _category_service_check(noteService);
            _categoryService = categoryService;
            _markdownService = markdownService;

            LoadCommand = new RelayCommand(async _ => await LoadDataAsync());
            NewNoteCommand = new RelayCommand(_ => CreateNewNote());
            SaveNoteCommand = new RelayCommand(async _ => await SaveNoteAsync());
            DeleteNoteCommand = new RelayCommand(async _ => await DeleteNoteAsync());
            AddCategoryCommand = new RelayCommand(async _ => await AddCategoryAsync());
            RemoveCategoryCommand = new RelayCommand(async _ => await RemoveCategoryAsync());

            ShowEditCommand = new RelayCommand(_ => IsPreviewMode = false);
            ShowPreviewCommand = new RelayCommand(_ => IsPreviewMode = true);

            SortByCategoryCommand = new RelayCommand(async _ => { _sortAscending = !_sortAscending; await ReloadNotesKeepSelectionAsync(SelectedNote?.Id ?? 0); });

            IsPreviewMode = false;

            _ = LoadDataAsync();
        }

        private void _category_service_check(INoteService s) { }

        public async Task LoadDataAsync()
        {
            Notes.Clear();
            Categories.Clear();

            var notes = await FetchNotesForCurrentFilterAsync(includeAll: true);
            var categories = await _categoryService.GetAllCategoriesAsync();

            foreach (var category in categories)
                Categories.Add(category);

            ApplySortAndLoad(notes);

            if (SelectedCategory != null)
            {
                await FilterNotesAsync();
                return;
            }

            if (SelectedNote == null && Notes.Count > 0)
                SelectedNote = Notes[0];

            UpdatePreview();
        }

        private async Task<List<Note>> FetchNotesForCurrentFilterAsync(bool includeAll = false)
        {
            if (SelectedCategory == null && includeAll)
            {
                return await _noteService.GetAllNotesAsync();
            }
            else if (SelectedCategory == null)
            {
                return await _noteService.GetAllNotesAsync();
            }
            else
            {
                return await _noteService.GetNotesByCategoryAsync(SelectedCategory.Id);
            }
        }

        private void ApplySortAndLoad(IEnumerable<Note> sourceNotes)
        {
            Notes.Clear();

            IOrderedEnumerable<Note> ordered;
            if (_sortAscending)
            {
                ordered = sourceNotes.OrderBy(n => n.Category?.Name ?? string.Empty).ThenBy(n => n.Title);
            }
            else
            {
                ordered = sourceNotes.OrderByDescending(n => n.Category?.Name ?? string.Empty).ThenBy(n => n.Title);
            }

            foreach (var note in ordered)
                Notes.Add(note);

            if (Notes.Count > 0)
                SelectedNote = Notes[0];
            else
                SelectedNote = null;

            UpdatePreview();
        }

        private async Task FilterNotesAsync()
        {
            var notes = await FetchNotesForCurrentFilterAsync();
            ApplySortAndLoad(notes);
        }

        private void CreateNewNote()
        {
            SelectedNote = new Note
            {
                Title = "New Note ✏️",
                Content = "# New Note ✏️",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CategoryId = SelectedCategory?.Id,
                Category = SelectedCategory
            };

            IsPreviewMode = false;
            UpdatePreview();
        }

        private async Task SaveNoteAsync()
        {
            if (SelectedNote == null)
                return;

            SelectedNote.UpdatedAt = DateTime.Now;

            if (SelectedNote.Id == 0)
            {
                await _noteService.AddNoteAsync(SelectedNote);
            }
            else
            {
                await _note_service_update_check(SelectedNote);
                await _noteService.UpdateNoteAsync(SelectedNote);
            }

            await ReloadNotesKeepSelectionAsync(SelectedNote.Id);
            UpdatePreview();
        }

        private Task _note_service_update_check(Note note)
        {
            return Task.CompletedTask;
        }

        private async Task DeleteNoteAsync()
        {
            if (SelectedNote == null)
                return;

            var noteId = SelectedNote.Id;

            if (noteId != 0)
                await _noteService.DeleteNoteAsync(noteId);

            await FilterNotesAsync();
            UpdatePreview();
        }

        private async Task ReloadNotesKeepSelectionAsync(int selectedId)
        {
            var notes = await FetchNotesForCurrentFilterAsync(includeAll: true);
            ApplySortAndLoad(notes);

            SelectedNote = null;

            foreach (var note in Notes)
            {
                if (note.Id == selectedId)
                {
                    SelectedNote = note;
                    break;
                }
            }

            if (SelectedNote == null && Notes.Count > 0)
                SelectedNote = Notes[0];
        }

        public void UpdatePreview()
        {
            if (SelectedNote == null)
            {
                PreviewHtml = "<html><body style='font-family:Segoe UI; padding:16px;'><p>No note selected.</p></body></html>";
                return;
            }

            PreviewHtml = _markdownService.ToHtml(SelectedNote.Content ?? string.Empty);
        }

        private void SelectedNote_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Note.Content) || e.PropertyName == nameof(Note.Title))
            {
                UpdatePreview();
            }
        }

        private async Task AddCategoryAsync()
        {
            var name = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter category name:",
                "Add Category",
                "");

            if (string.IsNullOrWhiteSpace(name))
                return;

            await _categoryService.AddCategoryAsync(new Category { Name = name });
            await LoadDataAsync();
        }

        private async Task RemoveCategoryAsync()
        {
            if (SelectedCategory == null)
                return;

            var result = System.Windows.MessageBox.Show(
                $"Are you sure you want to delete category '{SelectedCategory.Name}'?",
                "Confirm Delete",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                await _categoryService.DeleteCategoryAsync(SelectedCategory.Id);
                SelectedCategory = null;
                await LoadDataAsync();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}