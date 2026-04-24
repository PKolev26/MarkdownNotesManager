using MarkdownNotesManager.Core.Models;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MarkdownNotesManager.App.Services
{
    public static class MarkdownExporter
    {
        public static string GetSafeFileName(string? title) =>
            string.IsNullOrWhiteSpace(title) ? "note.md" : $"{SanitizeFileName(title)}.md";

        private static string SanitizeFileName(string name)
        {
            var invalid = new string(Path.GetInvalidFileNameChars());
            var invalidRe = new Regex($"[{Regex.Escape(invalid)}]+");
            var cleaned = invalidRe.Replace(name, "_").Trim();
            if (string.IsNullOrWhiteSpace(cleaned))
                return "note";
            return cleaned;
        }

        public static async Task ExportNoteAsync(Note note, string filePath)
        {
            if (note == null) throw new ArgumentNullException(nameof(note));
            var sb = new StringBuilder();

            sb.AppendLine("---");
            sb.AppendLine($"title: \"{EscapeYaml(note.Title)}\"");
            sb.AppendLine($"created_at: \"{note.CreatedAt:O}\"");
            sb.AppendLine($"updated_at: \"{note.UpdatedAt:O}\"");
            sb.AppendLine($"category: \"{EscapeYaml(note.Category?.Name ?? string.Empty)}\"");
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine(note.Content ?? string.Empty);

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            await File.WriteAllBytesAsync(filePath, bytes);
        }

        private static string EscapeYaml(string? s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return s.Replace("\"", "\\\"");
        }
    }
}