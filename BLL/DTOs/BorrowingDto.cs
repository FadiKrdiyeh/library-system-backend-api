using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class BorrowingDto
    {
        public Guid Id { get; set; }
        public UserDto User { get; set; }
        public Guid UserId { get; set; }
        public BookDto Book { get; set; }
        public Guid BookId { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}
