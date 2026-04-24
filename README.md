# Markdown Notes Manager

A desktop note-taking application built with **C#**, **WPF**, **.NET 8**, **Entity Framework Core**, **SQLite**, and **Markdig**.

Markdown Notes Manager allows users to create, edit, organize, preview, and export Markdown notes through a clean desktop interface.

---

## Features

- Create new Markdown notes
- Edit note title and content
- Live Markdown preview
- Organize notes by categories
- Add and remove categories
- Sort notes by category
- Export notes as `.md` files
- Local SQLite database storage
- Automatic database migration on startup
- Unit tests for commands, models, services, Markdown conversion, and ViewModel logic
- GitHub Actions workflow for build and test automation

---

## Tech Stack

- **Language:** C#
- **Framework:** .NET 8
- **UI:** WPF
- **Architecture:** MVVM-style structure
- **Database:** SQLite
- **ORM:** Entity Framework Core
- **Markdown rendering:** Markdig
- **Testing:** MSTest
- **CI/CD:** GitHub Actions

---

## Project Structure

```txt
MarkdownNotesManager/
│
├── MarkdownNotesManager/                  # WPF desktop application
│   ├── Commands/                          # RelayCommand and command logic
│   ├── Services/                          # App-level services, export logic
│   ├── ViewModels/                        # MainViewModel and UI logic
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── MainWindow.xaml
│   └── MarkdownNotesManager.App.csproj
│
├── MarkdownNotesManager.Core/             # Core domain layer
│   ├── Interfaces/                        # Repository and service interfaces
│   ├── Models/                            # Note and Category models
│   └── MarkdownNotesManager.Core.csproj
│
├── MarkdownNotesManager.Infrastructure/   # Data and infrastructure layer
│   ├── Data/                              # AppDbContext
│   ├── Migrations/                        # EF Core migrations
│   ├── Repositories/                      # Note and category repositories
│   ├── Services/                          # Note, category and Markdown services
│   └── MarkdownNotesManager.Infrastructure.csproj
│
├── MarkdownNotesManager.Tests/            # Unit tests
│   ├── UnitTests.cs
│   └── MarkdownNotesManager.Tests.csproj
│
├── .github/workflows/                     # GitHub Actions workflow
├── MarkdownNotesManager.sln
└── README.md
