using BLL.Models;
using DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class BookRepository : IBook
    {
        private readonly IConfiguration _configuration;

        public BookRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<Book>> GetBooks(string search)
        {
            try
            {
                List<Book> books = new List<Book>();
                SqlHandler sqlHandler = new SqlHandler(_configuration);
                string sqlStatment = @"
                    SELECT *
                        FROM Books
                        WHERE (Title LIKE '%" + search + @"%' OR Author LIKE '%" + search + @"%' OR ISBN LIKE '%" + search + @"%')
                        ORDER BY CreatedAt DESC;
                ";

                DataTable result = await sqlHandler.ExecReaderAsync(sqlStatment);

                foreach (DataRow row in result.Rows) {
                    Book book = new Book();
                    book.Id = Guid.Parse(Convert.ToString(row["Id"]));
                    book.Title = Convert.ToString(row["Title"]);
                    book.Description = Convert.ToString(row["Description"]);
                    book.ISBN = Convert.ToString(row["ISBN"]);
                    book.Author = Convert.ToString(row["Author"]);
                    book.Availability = Convert.ToInt32(row["Availability"]);
                    book.CreatedAt = Convert.ToDateTime(row["CreatedAt"]);
                    book.UpdatedAt = Convert.ToDateTime(row["UpdatedAt"]);

                    books.Add(book);
                    Console.WriteLine(row["Id"]);
                }

                return books;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Book> GetBook(string id)
        {
            try
            {
                SqlHandler sqlHandler = new SqlHandler(_configuration);
                string sqlStatment = @"
                    SELECT *
                        FROM Books
                        WHERE Id = '" + id + @"';
                ";

                DataTable result = await sqlHandler.ExecReaderAsync(sqlStatment);

                if (result.Rows.Count > 0) {
                    Console.WriteLine(Guid.Parse(Convert.ToString(result.AsDataView()[0]["Id"])));
                    return new Book
                    {
                        Id = Guid.Parse(Convert.ToString(result.AsDataView()[0]["Id"])),
                        Title = Convert.ToString(result.AsDataView()[0]["Title"]),
                        Description = Convert.ToString(result.AsDataView()[0]["Description"]),
                        ISBN = Convert.ToString(result.AsDataView()[0]["ISBN"]),
                        Availability = Convert.ToInt32(result.AsDataView()[0]["Availability"]),
                        Author = Convert.ToString(result.AsDataView()[0]["Author"]),
                        CreatedAt = Convert.ToDateTime(result.AsDataView()[0]["CreatedAt"]),
                        UpdatedAt = Convert.ToDateTime(result.AsDataView()[0]["UpdatedAt"]),
                    };
                } else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Book> CreateBook(Book book)
        {
            try
            {
                SqlHandler sqlHandler = new SqlHandler(_configuration);

                var datetime = DateTime.Now;

                string sqlStatment = @"
                    INSERT INTO Books
                           (Id,
                           Title,
                           Description,
                           ISBN,
                           Availability,
                           Author,
                           CreatedAt,
                           UpdatedAt)
                    output INSERTED.Id
                     VALUES
                           (NEWID(),
                           @Title,
                           @Description,
                           @ISBN,
                           @Availability,
                           @Author,
                           GETDATE(),
                           GETDATE())
                ";

                SqlParameter[] parameters = new SqlParameter[5];
                parameters[0] = new SqlParameter("@Title", book.Title);
                parameters[1] = new SqlParameter("@Description", book.Description);
                parameters[2] = new SqlParameter("@ISBN", book.ISBN);
                parameters[3] = new SqlParameter("@Availability", (book.Availability != null && book.Availability > -1 && book.Availability <= 2) ? book.Availability : 1);
                parameters[4] = new SqlParameter("@Author", book.Author);


                var result = (Guid) await sqlHandler.ExecCommandNonQueryAsync(sqlStatment, parameters, true);

                if (result != null) {
                    Book createdBook = await GetBook(result.ToString());
                    return createdBook;
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

        public async Task<bool> UpdateBook(Book book)
        {
            try
            {
                SqlHandler sqlHandler = new SqlHandler(_configuration);

                string sqlStatment = @"
                    UPDATE Books
                    SET Title = @Title,
                        Description = @Description,
                        ISBN = @ISBN,
                        Availability = @Availability,
                        Author = @Author,
                        UpdatedAt = GETDATE()
                    WHERE Id = @Id;
                ";

                SqlParameter[] parameters = new SqlParameter[6];
                parameters[0] = new SqlParameter("@Title", book.Title);
                parameters[1] = new SqlParameter("@Description", book.Description != null ? book.Description : "");
                parameters[2] = new SqlParameter("@ISBN", book.ISBN);
                parameters[3] = new SqlParameter("@Availability", book.Availability);
                parameters[4] = new SqlParameter("@Author", book.Author);
                parameters[5] = new SqlParameter("@Id", book.Id);

                var result = (bool) await sqlHandler.ExecCommandNonQueryAsync(sqlStatment, parameters);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteBook(string id)
        {
            try
            {
                SqlHandler sqlHandler = new SqlHandler(_configuration);
                string sqlStatment = @"
                    DELETE FROM Books
                        WHERE Id = @Id;
                "
                ;

                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@Id", id);

                var result = (bool)await sqlHandler.ExecCommandNonQueryAsync(sqlStatment, parameters);

                if (result)
                {
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
