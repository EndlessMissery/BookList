using System; // Import základních tříd a funkcí systému
using System.Collections.Generic; // Import pro práci s kolekcemi jako List a Dictionary
using System.IO; // Import pro práci se soubory
using System.Linq; // Import pro dotazování na kolekce (LINQ)
using Book_List.Utilities; // Import vlastních utilit pro formátování výstupu
using Spectre.Console; // Import pro práci s konzolovou grafikou a vstupy
using CsvHelper; // Import knihovny pro práci s CSV soubory

namespace Book_List.Models // Definice prostoru názvů, aby byly třídy a funkce organizovány
{
    public class Library // Deklarace třídy Library, která reprezentuje knihovnu
    {
        // Privátní pole pro databázový kontext, který zajišťuje přístup k databázi
        private AppDbContext _context;

        // Slovník žánrů a podžánrů knih
        private readonly Dictionary<string, List<string>> _genres = new Dictionary<string, List<string>>
        {
            { "Autobiografie", new List<string>() },
            { "Klasicka literatura", new List<string>() },
            { "Historie", new List<string>() },
            { "Roman", new List<string>() },
            { "Pravo", new List<string>() },
            { "Komiks", new List<string>() },
            { "Podle skutecnych udalosti", new List<string> { "Psychologie" } },
            { "Veda", new List<string> { "Ekonomika", "Matematika", "Fyzika", "Filozofie" } },
            { "Filozofie", new List<string> { "Veda", "Objektivismus", "Ekonomika", "Vzdelavani", "Eseje", "Psychologie" } },
            { "Technologie", new List<string> { "Matematika", "Data science", "Pocitacova veda", "Zpracovani signalu", "Ekonomika" } },
            { "Ostatni", new List<string>() },
        };

        // Metoda pro nastavení databázového kontextu
        public void SetContext(AppDbContext context)
        {
            _context = context; // Přiřazení předaného kontextu do privátního pole
        }

        // Metoda pro přidání nové knihy do databáze
        public void AddBook()
        {
            AnsiConsole.Clear(); // Vyčištění konzole
            AnsiConsole.Markup("Zadeje NÁZEV nové knihy: ");
            var nazevKnihy = Console.ReadLine(); // Získání názvu knihy od uživatele
            AnsiConsole.Markup("Zadejte JMÉNO a PŘÍJMENÍ autora knihy: ");
            var autorKnihy = Console.ReadLine(); // Získání jména autora od uživatele

            // Výběr žánru knihy z předdefinovaného seznamu
            var zanrKnihy = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Zadejte ŽÁNR knihy:")
                    .AddChoices(_genres.Keys));

            // Kontrola, zda zvolený žánr má podžánry, a jejich výběr
            if (_genres.ContainsKey(zanrKnihy) && _genres[zanrKnihy].Any())
            {
                var podzanrKnihy = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Zadejte PODŽÁNR knihy:")
                        .AddChoices(_genres[zanrKnihy]));
                zanrKnihy += $" - {podzanrKnihy}"; // Přidání podžánru k žánru
            }

            // Vytvoření nové knihy a přidání do databáze
            var book = new AppDbContext.Book { Title = nazevKnihy, Author = autorKnihy, Genre = zanrKnihy };
            _context.Books.Add(book);
            _context.SaveChanges(); // Uložení změn do databáze

            // Informování uživatele o úspěšném přidání knihy
            ConsoleStyles.WriteSuccess("Kniha přidána\n");
            ConsoleStyles.WaitForKeyPress(); // Čekání na stisk klávesy
        }

        // Metoda pro zobrazení seznamu knih
        public void BookList()
        {
            AnsiConsole.Clear(); // Vyčištění konzole
            var books = _context.Books.ToList(); // Načtení seznamu knih z databáze
            if (books.Count == 0)
            {
                ConsoleStyles.WriteError("Seznam je prázdný\n"); // Zobrazení chyby, pokud je seznam prázdný
            }
            else
            {
                ConsoleStyles.DisplayTable("Seznam knih", books); // Zobrazení tabulky knih
            }

            ConsoleStyles.WaitForKeyPress(); // Čekání na stisk klávesy
        }

        // Metoda pro odstranění knihy ze seznamu
        public void DeleteBook()
        {
            AnsiConsole.Clear(); // Vyčištění konzole
            if (!_context.Books.Any())
            {
                ConsoleStyles.WriteError("V seznamu nejsou žádné knihy\n"); // Zobrazení chyby, pokud nejsou žádné knihy
            }
            else
            {   
                // Výpis seznamu knih
                ConsoleStyles.WriteInfo("Seznam knih:");
                BookList(); // Zobrazení seznamu knih

                Console.Write("Zadejte název knihy a autora k odstranění díla ze seznamu (ve formátu 'Název knihy od Jméno autora'): ");
                var input = Console.ReadLine(); // Získání vstupu od uživatele

                // Rozdělení vstupu na název a autora
                if (input != null)
                {
                    var parts = input.Split(" od ");
                    if (parts.Length != 2)
                    {
                        ConsoleStyles.WriteError("Špatný formát vstupu\n"); // Zobrazení chyby, pokud je formát vstupu nesprávný
                    }
                    else
                    {
                        var title = parts[0];
                        var author = parts[1];

                        // Vyhledání knihy v databázi
                        var book = _context.Books.FirstOrDefault(b => b.Title == title && b.Author == author);

                        if (book != null)
                        {
                            // Odstranění knihy z databáze
                            _context.Books.Remove(book);
                            _context.SaveChanges(); // Uložení změn do databáze
                            ConsoleStyles.WriteSuccess("Kniha odstraněna\n"); // Informování uživatele o úspěšném odstranění knihy
                        }
                        else
                        {
                            ConsoleStyles.WriteError("Kniha nenalezena v seznamu\n"); // Zobrazení chyby, pokud kniha nebyla nalezena
                        }
                    }
                }
            }

            ConsoleStyles.WaitForKeyPress(); // Čekání na stisk klávesy
        }

        // Privátní metoda pro vyhledávání knih v databázi
        private List<AppDbContext.Book> SearchBooks(string searchTerm, string genre = null)
        {
            var query = _context.Books.AsQueryable(); // Vytvoření dotazu na knihy

            // Filtrování podle názvu nebo autora
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(b => b.Title.Contains(searchTerm) || b.Author.Contains(searchTerm));
            }

            // Filtrování podle žánru
            if (!string.IsNullOrEmpty(genre))
            {
                query = query.Where(b => b.Genre.Contains(genre));
            }

            return query.ToList(); // Vrácení výsledků vyhledávání jako seznam
        }

        // Veřejná metoda pro vyhledávání a zobrazení knih
        public void SearchAndDisplayBooks()
        {
            AnsiConsole.Clear(); // Vyčištění konzole
            var searchTerm = AnsiConsole.Ask<string>("Zadejte název knihy:"); // Získání názvu knihy od uživatele
            var authorFilter = AnsiConsole.Ask<string>("Zadejte autora knihy:"); // Získání autora od uživatele
            var genreFilter = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Zvolte žánr knihy:")
                    .AddChoices(_genres.Keys)); // Získání žánru od uživatele

            var result = SearchBooks(searchTerm, genreFilter); // Vyhledání knih

            // Další filtrování podle autora
            if (!string.IsNullOrWhiteSpace(authorFilter))
            {
                result = result.Where(b => b.Author.Contains(authorFilter)).ToList();
            }

            // Zobrazení výsledků nebo chyby, pokud žádné knihy nebyly nalezeny
            if (result.Count == 0)
            {
                ConsoleStyles.WriteError("Kniha nenalezena\n");
            }
            else
            {
                ConsoleStyles.DisplayTable("Výsledky hledání", result); // Zobrazení tabulky s výsledky hledání
            }

            ConsoleStyles.WaitForKeyPress(); // Čekání na stisk klávesy
        }

        // Metoda pro filtrování knih podle autora nebo žánru
        public void FilterBooks()
        {
            AnsiConsole.Clear(); // Vyčištění konzole

            // Výběr typu filtru (autor nebo žánr)
            var filterType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Vyberte filtr:")
                    .PageSize(13)
                    .AddChoices(new List<string> { "Podle autora", "Podle žánru" }));

            switch (filterType)
            {
                case "Podle autora":
                    // Výběr autora ze seznamu
                    var selectedAuthor = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Vyberte autora:")
                            .PageSize(13)
                            .AddChoices(_context.Books.Select(b => b.Author).Distinct().ToList()));

                    // Vyhledání knih podle autora
                    var authorResults = _context.Books.Where(b => b.Author == selectedAuthor).ToList();

                    // Zobrazení výsledků nebo chyby, pokud žádné knihy nebyly nalezeny
                    if (authorResults.Count == 0)
                    {
                        ConsoleStyles.WriteError("Kniha nenalezena\n");
                    }
                    else
                    {
                        ConsoleStyles.DisplayTable($"Výsledky hledání podle autora: {selectedAuthor}", authorResults);
                    }
                    break;

                case "Podle žánru":
                    // Výběr žánru ze seznamu
                    var genreFilter = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Zadejte žánr knihy:")
                            .PageSize(13)
                            .AddChoices(_genres.Keys));

                    // Vyhledání knih podle žánru
                    var genreResults = _context.Books.Where(b => b.Genre.Contains(genreFilter)).ToList();

                    // Zobrazení výsledků nebo chyby, pokud žádné knihy nebyly nalezeny
                    if (genreResults.Count == 0)
                    {
                        ConsoleStyles.WriteError("Kniha nenalezena\n");
                    }
                    else
                    {
                        ConsoleStyles.DisplayTable($"Výsledky hledání podle žánru: {genreFilter}", genreResults);
                    }
                    break;

                default:
                    ConsoleStyles.WriteError("Neplatná volba\n"); // Zobrazení chyby, pokud byla zvolena neplatná volba
                    break;
            }

            ConsoleStyles.WaitForKeyPress(); // Čekání na stisk klávesy
        }

        // Metoda pro import knih z CSV souboru
        public void ImportCsv()
        {
            AnsiConsole.Clear(); // Vyčištění konzole

            var filePath = AnsiConsole.Ask<string>("Zadejte cestu k CSV souboru:"); // Získání cesty k CSV souboru od uživatele

            try
            {
                using var reader = new StreamReader(filePath); // Vytvoření StreamReaderu pro čtení souboru
                using var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture); // Vytvoření CsvReaderu pro čtení CSV dat
                var records = csv.GetRecords<BookCsvModel>().ToList(); // Načtení záznamů z CSV souboru do seznamu
                var newBooks = new List<AppDbContext.Book>(); // Seznam nových knih

                // Iterace přes záznamy a přidání knih, které nejsou v databázi
                foreach (var record in records)
                {
                    if (!_context.Books.Any(b => b.Title == record.Title && b.Author == record.Author && b.Genre == record.Genre && b.Subgenre == record.Subgenre))
                    {
                        var book = new AppDbContext.Book
                        {
                            Title = record.Title,
                            Author = record.Author,
                            Genre = record.Genre,
                            Subgenre = record.Subgenre
                        };
                        newBooks.Add(book); // Přidání nové knihy do seznamu
                    }
                }

                _context.Books.AddRange(newBooks); // Přidání nových knih do databáze
                _context.SaveChanges(); // Uložení změn do databáze

                ConsoleStyles.WriteSuccess($"CSV soubor byl úspěšně importován. Přidáno {newBooks.Count} nových knih.\n"); // Informování uživatele o úspěšném importu
            }
            catch (Exception ex)
            {
                ConsoleStyles.WriteError($"Chyba při importu CSV souboru: {ex.Message}\n"); // Zobrazení chyby, pokud nastane výjimka
            }

            ConsoleStyles.WaitForKeyPress(); // Čekání na stisk klávesy
        }

        // Metoda pro export knih do CSV souboru
        public void ExportCsv()
        {
            AnsiConsole.Clear(); // Vyčištění konzole

            var filePath = AnsiConsole.Ask<string>("Zadejte název pro CSV soubor:"); // Získání názvu souboru od uživatele

            try
            {
                using var writer = new StreamWriter(filePath); // Vytvoření StreamWriteru pro zápis do souboru
                using var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture); // Vytvoření CsvWriteru pro zápis CSV dat
                csv.WriteRecords(_context.Books.ToList()); // Zápis záznamů knih do CSV souboru

                ConsoleStyles.WriteSuccess($"Data byla úspěšně exportována do CSV souboru.\n"); // Informování uživatele o úspěšném exportu
            }
            catch (Exception ex)
            {
                ConsoleStyles.WriteError($"Chyba při exportu CSV souboru: {ex.Message}\n"); // Zobrazení chyby, pokud nastane výjimka
            }

            ConsoleStyles.WaitForKeyPress(); // Čekání na stisk klávesy
        }
    }
}
