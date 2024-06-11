using System.ComponentModel.DataAnnotations;

namespace Book_List.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string Název { get; set; }
        public string Autor { get; set; }
        public string Žánr { get; set; } 
        public string Podžánr { get; set; }
    }
}
