using BLL.IRepository;
using BLL.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System;
using DAL.Interfaces;
using System.Data;
using BLL.DTOs;
using System.Collections.Generic;

namespace DAL.Repository
{
    public class BorrowingRepository : IBorrowing
    {
        private readonly IConfiguration _configuration;
        private readonly IBook _bookRepository;

        public BorrowingRepository(IConfiguration configuration, IBook bookRepository)
        {
            _configuration = configuration;
            _bookRepository = bookRepository;
        }

        public async Task<Borrowing> GetBorrowing(string id)
        {
            try
            {
                SqlHandler sqlHandler = new SqlHandler(_configuration);
                string sqlStatment = @"
                    SELECT *
                        FROM Borrowings
                        WHERE Id = '" + id + @"';
                ";

                DataTable result = await sqlHandler.ExecReaderAsync(sqlStatment);

                if (result.Rows.Count > 0)
                {
                    return new Borrowing
                    {
                        Id = Guid.Parse(Convert.ToString(result.AsDataView()[0]["Id"])),
                        BookId = Convert.ToString(result.AsDataView()[0]["BookId"]),
                        UserId = Convert.ToString(result.AsDataView()[0]["UserId"]),
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<BorrowingDto>> GetBorrowings(string userId)
        {
            try
            {
                SqlHandler sqlHandler = new SqlHandler(_configuration);

                List<BorrowingDto> borrowedBooks = new List<BorrowingDto>();

                string sqlStatment = @"SELECT br.Id, br.CreatedAt, b.Id AS BookId, b.Title, b.ISBN, b.Availability, b.Author, u.Id AS UserId, u.Name
                                FROM Books b
                                INNER JOIN Borrowings br ON b.Id = br.BookId
                                INNER JOIN Users u ON br.UserId = u.Id
                                WHERE br.UserId = '" + userId + "'";

                DataTable result = await sqlHandler.ExecReaderAsync(sqlStatment);

                foreach (DataRow row in result.Rows)
                {
                    BookDto book = new BookDto();
                    book.Id = Guid.Parse(Convert.ToString(row["BookId"]));
                    book.Title = Convert.ToString(row["Title"]);
                    book.ISBN = Convert.ToString(row["ISBN"]);
                    book.Author = Convert.ToString(row["Author"]);
                    book.Availability = Convert.ToInt32(row["Availability"]);

                    UserDto user = new UserDto();
                    user.Id = Guid.Parse(Convert.ToString(row["UserId"]));
                    user.Name = Convert.ToString(row["Name"]);

                    BorrowingDto borrowedBook = new BorrowingDto();
                    borrowedBook.Id = Guid.Parse(Convert.ToString(row["Id"]));
                    borrowedBook.UserId = Guid.Parse(Convert.ToString(row["UserId"]));
                    borrowedBook.Book = book;
                    borrowedBook.BookId = Guid.Parse(Convert.ToString(row["BookId"]));
                    borrowedBook.User = user;
                    borrowedBook.CreatedAt = Convert.ToDateTime(row["CreatedAt"]).ToString();


                    borrowedBooks.Add(borrowedBook);
                }

                return borrowedBooks;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Borrowing> BorrowBook(Borrowing borrowing)
        {
            try
            {
                SqlHandler sqlHandler = new SqlHandler(_configuration);

                Book book = await _bookRepository.GetBook(borrowing.BookId);

                if (book.Availability == 1)
                {
                    string sqlStatment = @"
                    INSERT INTO Borrowings
                           (Id,
                           BookId,
                           UserId,
                           CreatedAt,
                           UpdatedAt)
                    output INSERTED.Id
                     VALUES
                           (NEWID(),
                           @BookId,
                           @UserId,
                           GETDATE(),
                           GETDATE())
                ";

                    SqlParameter[] parameters = new SqlParameter[2];
                    parameters[0] = new SqlParameter("@BookId", borrowing.BookId);
                    parameters[1] = new SqlParameter("@UserId", borrowing.UserId);


                    var result = (Guid)await sqlHandler.ExecCommandNonQueryAsync(sqlStatment, parameters, true);

                    if (result != null)
                    {
                        book.Availability = 0;
                        var updateBookAvailability = await _bookRepository.UpdateBook(book);
                        borrowing.Id = result;
                        return borrowing;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    borrowing.Id = Guid.Empty;
                    return borrowing;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> ReturnBook(Borrowing borrowing)
        {
            try
            {
                SqlHandler sqlHandler = new SqlHandler(_configuration);
                string sqlStatment = @"
                    DELETE FROM Borrowings
                        WHERE Id = @Id;
                ";

                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@Id", borrowing.Id);

                var result = (bool)await sqlHandler.ExecCommandNonQueryAsync(sqlStatment, parameters);

                if (result)
                {
                    Book book = await _bookRepository.GetBook(borrowing.BookId);
                    book.Availability = 1;
                    var updateBookAvailability = await _bookRepository.UpdateBook(book);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
