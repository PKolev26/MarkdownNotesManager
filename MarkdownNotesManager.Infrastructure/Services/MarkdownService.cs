using Markdig;

namespace MarkdownNotesManager.Infrastructure.Services
{
    public class MarkdownService
    {
        public string ToHtml(string markdown)
        {
            var body = Markdown.ToHtml(markdown ?? string.Empty);

            return $$"""
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset="utf-8" />
                    <style>
                        body {
                            font-family: Segoe UI, Arial, sans-serif;
                            padding: 20px;
                            color: #111827;
                            line-height: 1.6;
                            background: white;
                        }

                        h1, h2, h3 {
                            margin-top: 24px;
                            color: #111827;
                        }

                        code {
                            background: #F3F4F6;
                            padding: 2px 6px;
                            border-radius: 4px;
                            font-family: Consolas, monospace;
                        }

                        pre {
                            background: #F3F4F6;
                            padding: 12px;
                            border-radius: 6px;
                            overflow-x: auto;
                        }

                        blockquote {
                            border-left: 4px solid #D1D5DB;
                            margin: 16px 0;
                            padding-left: 12px;
                            color: #4B5563;
                        }

                        table {
                            border-collapse: collapse;
                            width: 100%;
                        }

                        th, td {
                            border: 1px solid #D1D5DB;
                            padding: 8px;
                            text-align: left;
                        }
                    </style>
                </head>
                <body>
                    {{body}}
                </body>
                </html>
                """;
        }
    }
}