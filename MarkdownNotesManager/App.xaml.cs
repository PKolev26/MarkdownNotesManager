using MarkdownNotesManager.App.ViewModels;
using MarkdownNotesManager.Core.Interfaces;
using MarkdownNotesManager.Infrastructure.Repositories;
using MarkdownNotesManager.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace MarkdownNotesManager.App
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; }

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<INoteRepository, NoteRepository>();
            services.AddSingleton<ICategoryRepository, CategoryRepository>();

            services.AddSingleton<INoteService, NoteService>();
            services.AddSingleton<ICategoryService, CategoryService>();

            services.AddSingleton<MarkdownService>();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }
    }
}