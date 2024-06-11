using CsvHelper.Configuration.Attributes;

namespace Book_List.Models
{
    public class BookCsvModel
    {
        [Name("Id")]
        public int Id { get; set; }

        [Name("Title")]
        public string Title { get; set; }

        [Name("Author")]
        public string Author { get; set; }

        [Name("Genre")]
        public string Genre { get; set; }
        
        [Name("Subgenre")]
        public string Subgenre { get; set; }
    }
}