using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Models;

namespace DAL.Interfaces
{
    public interface IBook
    {
        Task<List<Book>> GetBooks(string search);
        Task<Book> GetBook(string id);
        Task<Book> CreateBook(Book book);
        Task<bool> UpdateBook(Book book);
        Task<bool> DeleteBook(string id);
    }
}
