using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig;

namespace MarkdownNotesManager.Infrastructure.Services
{
    public class MarkdownService
    {
        public string ToHtml(string markdown)
        {
            return Markdown.ToHtml(markdown ?? string.Empty);
        }
    }
}
