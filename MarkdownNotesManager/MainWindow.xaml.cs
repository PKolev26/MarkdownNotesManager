using MarkdownNotesManager.App.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace MarkdownNotesManager.App
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;

            RenderPreview();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.PreviewHtml))
            {
                RenderPreview();
            }
        }

        private void RenderPreview()
        {
            var html = _viewModel.PreviewHtml;

            if (string.IsNullOrWhiteSpace(html))
            {
                html = "<html><body></body></html>";
            }

            PreviewBrowser.NavigateToString(html);
        }
    }
}