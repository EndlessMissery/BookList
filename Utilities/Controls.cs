using Spectre.Console;
using Book_List.Models;

namespace Book_List.Utilities
{
    public class Controls
    {
        private Library _library;

        // Metoda pro library
        public void SetLibrary(Library library)
        {
            _library = library;
        }

        public void MainMenu()
        {
            var isSelected = false;

            while (!isSelected)
            {
                AnsiConsole.Clear();
                var selection = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .PageSize(10)
                        .Title(
                            "Pro orientaci v Menu využijte směrové šipky nahoru a dolů, pro potvrzení výběru stiskněte [yellow]ENTER[/]")
                        .AddChoices("Přidat knihu", "Seznam knih", "Hledat knihu", "Filtrovat knihy",
                            "Importovat seznam (.csv)", "Exportovat seznam (.csv)", "Odstranit knihu",
                            "Ukončit knihovnu"));

                switch (selection)
                {
                    case "Přidat knihu":
                        _library.AddBook();
                        break;

                    case "Seznam knih":
                        _library.BookList();
                        break;

                    case "Hledat knihu":
                        _library.SearchAndDisplayBooks();
                        break;

                    case "Filtrovat knihy":
                        _library.FilterBooks();
                        break;

                    case "Importovat seznam (.csv)":
                        _library.ImportCsv();
                        break;

                    case "Exportovat seznam (.csv)":
                        _library.ExportCsv();
                        break;

                    case "Odstranit knihu":
                        _library.DeleteBook();
                        break;

                    case "Ukončit knihovnu":
                        isSelected = true;
                        break;
                }
            }
        }
    }
}

