using BLL.DTOs;
using BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.IRepository
{
    public interface IBorrowing
    {
        Task<Borrowing> GetBorrowing(string id);
        Task<List<BorrowingDto>> GetBorrowings(string userId);
        Task<Borrowing> BorrowBook(Borrowing borrowing);
        Task<bool> ReturnBook(Borrowing borrowing);
    }
}
