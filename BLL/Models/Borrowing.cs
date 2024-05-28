using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class Borrowing
    {

        [Required]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey("Book")]
        public string BookId { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
