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
        private string _previewHtml = string.Empty;

        public ObservableCollection<Note> Notes { get; set; } = new();
        public ObservableCollection<Category> Categories { get; set; } = new();

        public Note? SelectedNote
        {
            get => _selectedNote;
            set
            {
                _selectedNote = value;
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

        public ICommand LoadCommand { get; }
        public ICommand NewNoteCommand { get; }
        public ICommand SaveNoteCommand { get; }
        public ICommand DeleteNoteCommand { get; }

        public MainViewModel(
            INoteService noteService,
            ICategoryService categoryService,
            MarkdownService markdownService)
        {
            _noteService = noteService;
            _categoryService = categoryService;
            _markdownService = markdownService;

            LoadCommand = new RelayCommand(async _ => await LoadDataAsync());
            NewNoteCommand = new RelayCommand(_ => CreateNewNote());
            SaveNoteCommand = new RelayCommand(async _ => await SaveNoteAsync());
            DeleteNoteCommand = new RelayCommand(async _ => await DeleteNoteAsync());

            _ = LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            Notes.Clear();
            Categories.Clear();

            var notes = await _noteService.GetAllNotesAsync();
            var categories = await _categoryService.GetAllCategoriesAsync();

            foreach (var note in notes)
                Notes.Add(note);

            foreach (var category in categories)
                Categories.Add(category);

            if (SelectedNote == null && Notes.Count > 0)
                SelectedNote = Notes[0];
        }

        private void CreateNewNote()
        {
            SelectedNote = new Note
            {
                Title = "New Note",
                Content = "# New Note",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            UpdatePreview();
        }

        private async Task SaveNoteAsync()
        {
            if (SelectedNote == null)
                return;

            if (SelectedNote.Id == 0)
            {
                await _noteService.AddNoteAsync(SelectedNote);
                Notes.Add(SelectedNote);
            }
            else
            {
                await _noteService.UpdateNoteAsync(SelectedNote);
            }

            UpdatePreview();
        }

        private async Task DeleteNoteAsync()
        {
            if (SelectedNote == null)
                return;

            if (SelectedNote.Id != 0)
                await _noteService.DeleteNoteAsync(SelectedNote.Id);

            Notes.Remove(SelectedNote);
            SelectedNote = Notes.Count > 0 ? Notes[0] : null;
            UpdatePreview();
        }

        public void UpdatePreview()
        {
            if (SelectedNote == null)
            {
                PreviewHtml = "<html><body><p>No note selected.</p></body></html>";
                return;
            }

            PreviewHtml = _markdownService.ToHtml(SelectedNote.Content ?? string.Empty);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}