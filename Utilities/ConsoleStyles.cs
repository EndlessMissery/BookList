using Spectre.Console;
using System;
using System.Collections.Generic;

namespace Book_List.Utilities
{
    public static class ConsoleStyles
    {
        public static void WriteError(string message)
        {
            AnsiConsole.MarkupLine($"[bold red]ERROR:[/]: {message}");
        }

        public static void WriteSuccess(string message)
        {
            AnsiConsole.MarkupLine($"[bold green]{message}[/]");
        }

        public static void WriteInfo(string message)
        {
            AnsiConsole.MarkupLine($"[bold blue]{message}[/]");
        }

        public static void WaitForKeyPress(string message = "Pro návrat stiskněte libovlnou klávesu")
        {
            AnsiConsole.MarkupLine($"[bold yellow]{message}[/]");
            Console.ReadKey(true);
        }

        public static void DisplayTable<T>(string title, List<T> items)
        {
            var table = new Table().Border(TableBorder.Rounded)
                .BorderColor(Color.Grey)
                .Centered()
                .Title($"[bold yellow]{title}[/]");

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                table.AddColumn(GetColumnName(property.Name));
            }

            foreach (var item in items)
            {
                var row = new List<string>();
                foreach (var property in properties)
                {
                    var value = property.GetValue(item)?.ToString() ?? string.Empty;
                    row.Add(value);
                }

                table.AddRow(row.ToArray());
            }

            AnsiConsole.Write(table);
        }

        private static string GetColumnName(string propertyName)
        {
            switch (propertyName)
            {
                case "Title": return "[bold]Název[/]";
                case "Author": return "[bold]Autor[/]";
                case "Genre": return "[bold]Žánr[/]";
                case "Subgenre": return "[bold]Podžánr[/]";
                default: return propertyName;
            }
        }

    }
}
