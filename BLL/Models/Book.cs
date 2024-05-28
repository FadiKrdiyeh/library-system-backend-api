using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.Models
{
    public class Book
    {
        [Required]
        //[StringLength(100, MinimumLength = 3)]
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        //[RegularExpression(@"(?:ISBN(?:-13)?\:?)(?\d{13})|(?:ISBN(?:-10)?\:)?(?\d{10})", ErrorMessage = "Invalid ISBN format")]
        public string ISBN { get; set; }
        [Range(0, 2)]
        public int Availability { get; set; }
        [Required]
        public string Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
